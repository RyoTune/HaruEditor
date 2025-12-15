using System;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HaruEditor.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Window = ShadUI.Window;

namespace HaruEditor.Desktop.Views;

[IViewFor<MainWindowViewModel>]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (Random.Shared.Next(10) == 0)
        {
            const int numAlts = 2;
            var altId = Random.Shared.Next(numAlts) + 1;
            Icon = new(new Bitmap(AssetLoader.Open(new($"avares://HaruEditor.Desktop/Assets/icon-alt-{altId}.ico"))));
        }
        
        this.WhenActivated(disp =>
        {
            ViewModel?.SelectFolder.RegisterHandler(SelectFolderHandler).DisposeWith(disp);
        });
    }

    private async Task SelectFolderHandler(IInteractionContext<Unit, string?> obj)
    {
        var selectDir = await StorageProvider.OpenFolderPickerAsync(new() { Title = "Select Folder" });
        obj.SetOutput(selectDir.Count < 1 ? null : selectDir[0].Path.LocalPath);
    }
}