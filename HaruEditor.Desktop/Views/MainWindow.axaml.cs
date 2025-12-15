using System;
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
            Icon = new(new Bitmap(AssetLoader.Open(new($"avares://Haru Editor/Assets/icon-alt-{altId}.ico"))));
        }

        this.WhenActivated(_ =>
        {
        });
    }
}