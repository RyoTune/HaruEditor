using HaruEditor.Core.ModDiscovery.Reloaded;

namespace HaruEditor.Library.Tests.ModDiscovery.Reloaded;

public class ReloadedModDiscovererTests
{
    [Fact]
    public void ReloadedModDiscoverer_Works()
    {
        var reloadedDisc = new ReloadedModDiscoverer(null);
        Assert.NotEmpty(reloadedDisc.Mods);
    }
}