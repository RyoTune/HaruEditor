using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using HaruEditor.Core.ModDiscovery.Reloaded.Models;
using Microsoft.Extensions.Logging;

namespace HaruEditor.Core.ModDiscovery.Reloaded;

public class ReloadedModDiscoverer
{
    private readonly ILogger<ReloadedModDiscoverer>? _log;
    private readonly List<ModConfig> _mods = new();

    public ReloadedModDiscoverer(ILogger<ReloadedModDiscoverer>? log)
    {
        _log = log;
        
        DiscoverMods();
    }

    public IReadOnlyList<ModConfig> Mods => _mods;

    private void DiscoverMods()
    {
        if (TryGetModsDir(out var modsDir))
        {
            if (!Directory.Exists(modsDir)) return;

            foreach (var modDir in Directory.EnumerateDirectories(modsDir))
            {
                var modConfigFile = Path.Join(modDir, "ModConfig.json");
                if (!File.Exists(modConfigFile)) continue;

                try
                {
                    var modConfig = JsonSerializer.Deserialize<ModConfig>(File.ReadAllBytes(modConfigFile)) ?? throw new();
                    _mods.Add(modConfig);
                }
                catch (Exception ex) { _log?.LogError(ex, "Failed to load mod config."); }
            }
        }
    }

    private static bool TryGetModsDir([NotNullWhen(true)]out string? modsDir)
    {
        modsDir = Environment.GetEnvironmentVariable("RELOADEDIIMODS", EnvironmentVariableTarget.User);
        return modsDir != null;
    }
}