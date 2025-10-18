using System;
using System.Globalization;
using Avalonia.Data.Converters;
using HaruEditor.Desktop.Localization;

namespace HaruEditor.Desktop.Converters;

public class LocalizedObjectConverter : IValueConverter
{
    public static readonly LocalizedObjectConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ObjectLocalizer.GetValue(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}