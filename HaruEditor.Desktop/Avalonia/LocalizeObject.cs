using System;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using HaruEditor.Desktop.Converters;

namespace HaruEditor.Desktop.Avalonia;

public class LocalizeObject : MarkupExtension
{
    private readonly CompiledBindingExtension _binding;
    
    public LocalizeObject(CompiledBindingExtension binding)
    {
        _binding = binding;
        _binding.Converter = LocalizedObjectConverter.Instance;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider) => _binding;
}