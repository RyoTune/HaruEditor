using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
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