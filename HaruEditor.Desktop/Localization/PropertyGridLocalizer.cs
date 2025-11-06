using System;
using PropertyModels.ComponentModel;
using PropertyModels.Localization;
using ReactiveUI;
using ReactiveObject = ReactiveUI.ReactiveObject;

namespace HaruEditor.Desktop.Localization;

public class PropertyGridLocalizer : ReactiveObject, ILocalizationService
{
    public static readonly PropertyGridLocalizer Instance = new();
    
    void INotifyPropertyChanged.RaisePropertyChanged(string propertyName) => IReactiveObjectExtensions.RaisePropertyChanged(this, propertyName);

    public ILocalizationService[] GetExtraServices()
    {
        throw new NotImplementedException();
    }

    public void AddExtraService(ILocalizationService service)
    {
        throw new NotImplementedException();
    }

    public void RemoveExtraService(ILocalizationService service)
    {
        throw new NotImplementedException();
    }

    public ICultureData[] GetCultures()
    {
        throw new NotImplementedException();
    }

    public void SelectCulture(string cultureName)
    {
        throw new NotImplementedException();
    }

    public ICultureData CultureData { get; }

    public string this[string key] => ObjectLocalizer.GetValue(key);

    public event EventHandler? OnCultureChanged;
}