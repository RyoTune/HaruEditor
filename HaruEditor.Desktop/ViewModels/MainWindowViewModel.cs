using System;
using System.Reactive.Disposables.Fluent;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    [Reactive] private string? _currentRoute;
    [Reactive] private ViewModelBase? _currentContent;
    
    public MainWindowViewModel()
    {
        this.WhenActivated(disp =>
        {
            this.WhenValueChanged(x => x.CurrentRoute)
                .Subscribe(route =>
                {
                    CurrentContent = route switch
                    {
                        "editor" => new EditorViewModel(),
                        _ => (ViewModelBase?)null
                    };
                }).DisposeWith(disp);
        });
    }

    [ReactiveCommand]
    private void ChangeRoute(string newRoute) => CurrentRoute = newRoute;
}