using System.Diagnostics.CodeAnalysis;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using HaruEditor.Core.GameDiscovery.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

// ReSharper disable StringLiteralTypo
// ReSharper disable StringLiteralTypo

namespace HaruEditor.Core.GameDiscovery;

public class GameDiscoverer
{
    private readonly ILogger<GameDiscoverer>? _log;
    private readonly Dictionary<int, string> _gameInstalls = [];
    
    public GameDiscoverer(ILogger<GameDiscoverer>? log)
    {
        _log = log;

        try
        {
            DiscoverSteamGames();
        }
        catch (Exception ex) { log?.LogError(ex, "Failed to discover Steam games."); }
    }

    public bool TryGetSteamGameInstall(int appId, [NotNullWhen(true)]out string? gameInstallDir)
    {
        _gameInstalls.TryGetValue(appId, out gameInstallDir);
        return gameInstallDir != null;
    }

    private void DiscoverSteamGames()
    {
        if (!TryGetSteamInstall(out var steamDir)) return;
        
        var libFoldersFile = Path.Join(steamDir, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(libFoldersFile)) return;

        var libraries = VdfConvert.Deserialize(File.ReadAllText(libFoldersFile)).Value.ToJson().ToObject<Dictionary<string, SteamLibrary>>()!;
        foreach (var lib in libraries)
        {
            foreach (var app in lib.Value.Apps)
            {
                var appAcfFile = Path.Join(lib.Value.Path, "steamapps", $"appmanifest_{app.Key}.acf");
                if (!File.Exists(appAcfFile)) continue;

                var appAcf = VdfConvert.Deserialize(File.ReadAllText(appAcfFile)).Value.ToJson().ToObject<AppManifest>()!;
                var appDir = Path.Join(lib.Value.Path, "steamapps", "common", appAcf.InstallDir);
                if (!Directory.Exists(appDir)) continue;
                
                _gameInstalls[int.Parse(app.Key)] = appDir;
            }
        }
    }

    private static bool TryGetSteamInstall([NotNullWhen(true)]out string? steamDir)
    {
        steamDir = null;
        if (OperatingSystem.IsWindows())
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam") 
                            ?? Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");

            steamDir = key?.GetValue("InstallPath") as string;
        }

        return steamDir != null;
    }
}