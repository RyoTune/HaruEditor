using HaruEditor.Library.FileProviders.P5R;

namespace HaruEditor.Library.Tests.FileProviders.P5R;

public class FileProviderTests
{
    [Fact]
    public void FileProvider_TryGetFile_Works()
    {
        var prov = new FileProvider(null, new(null));
        Assert.True(prov.TryGetFile("base.cpk/battle/table/skill.tbl", out _));
    }
    
    [Fact]
    public void FileProvider_TryGetFiles_Works()
    {
        var prov = new FileProvider(null, new(null));
        Assert.True(prov.TryGetFiles(["base.cpk/battle/table/skill.tbl", "base.cpk/battle/table/encount.tbl"], out _));
    }
}