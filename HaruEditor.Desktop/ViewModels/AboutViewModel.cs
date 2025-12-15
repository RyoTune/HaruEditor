using System.Diagnostics;
using System.Reflection;

namespace HaruEditor.Desktop.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        Version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion ?? "Unknown";
    }

    public string Version { get; }
}