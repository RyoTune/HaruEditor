using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Avalonia.Controls;
using HaruEditor.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace HaruEditor.Desktop.Views;

public partial class EditorView : ReactiveUserControl<EditorViewModel>
{
    public EditorView()
    {
        InitializeComponent();
        
        this.WhenActivated(disp =>
        {
            ViewModel?.SelectFolder.RegisterHandler(SelectFolderHandler).DisposeWith(disp);
        });
    }

    private async Task SelectFolderHandler(IInteractionContext<Unit, string?> obj)
    {
        var selectDir = await TopLevel.GetTopLevel(this)!.StorageProvider.OpenFolderPickerAsync(new() { Title = "Select Folder" });
        obj.SetOutput(selectDir.Count < 1 ? null : selectDir[0].Path.LocalPath);
    }
}