using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace HaruEditor.Desktop.Controls;

public class HaruPropertyGrid : PropertyGrid
{
    public HaruPropertyGrid()
    {
        Factories.AddFactory(EnumAutocompleteFactory.Instance);
    }
}

public class EnumAutocompleteFactory : AbstractCellEditFactory
{
    public static readonly EnumAutocompleteFactory Instance = new();
    
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propDesc = context.Property;
        
        if (!propDesc.PropertyType.IsEnum || propDesc.Attributes.OfType<FlagsAttribute>().Any())
            return null;

        var values = Enum.GetValues(propDesc.PropertyType);
        if (values.Length < 20) return null;
        
        var control = new AutoCompleteBox
        {
            FilterMode = AutoCompleteFilterMode.ContainsOrdinal,
            ItemsSource = values,
            ValueMemberBinding = new Binding(".") { Converter = new EnumValueConverter(propDesc.PropertyType) },
            MinimumPrefixLength = 0,
        };
        
        control.SelectionChanged += (sender, _) =>
        {
            if (sender is AutoCompleteBox { SelectedItem: not null } acb)
            {
                SetAndRaise(context, acb, acb.SelectedItem);
            }
        };
        
        control.LostFocus += (sender, _) =>
        {
            if (sender is not AutoCompleteBox { SelectedItem: null } acb) return;
            
            // If the selection is left incomplete, such as a partial text search, default
            // to first value in the enum.
            var enumerator = acb.ItemsSource!.GetEnumerator();
            using var disp = enumerator as IDisposable;
            
            if (enumerator.MoveNext())
            {
                SetAndRaise(context, acb, enumerator.Current);

                // While the ACB's SelectedItem may be null, the actual property can not be.
                // This causes issues when selection is incomplete so we default to the first value,
                // but the property was already that. Nothing technically changed, meaning no notification to update,
                // meaning SelectedItem remains null in the ACB.
                // In that case, just manually set the SelectedItem.
                acb.SelectedItem ??= enumerator.Current;
            }
        };

        return control;
    }

    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        if (!propertyDescriptor.PropertyType.IsEnum)
        {
            return false;
        }
        
        var target = context.Target;
        var control = context.CellEdit!;
        
        ValidateProperty(control, propertyDescriptor, target);
        
        if (control is AutoCompleteBox acb)
        {
            var value = propertyDescriptor.GetValue(target);
            acb.SelectedItem = value;
            return true;
        }
        
        return false;
    }

    private class EnumValueConverter(Type enumType) : IValueConverter
    {
        private readonly Type _underlyingType = Enum.GetUnderlyingType(enumType);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not null)
            {
                return $"{System.Convert.ChangeType(value, _underlyingType)}. {value}";
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
