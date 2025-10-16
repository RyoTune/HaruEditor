// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CollectionNeverUpdated.Global
namespace HaruEditor.Core.GameDiscovery.Models;

public class SteamLibrary
{
    public string Path { get; set; } = string.Empty;
    public Dictionary<string, string> Apps { get; set; } = [];
}