using HaruEditor.Core.Tables.P5R;

namespace HaruEditor.Library.Tests.Tables.P5R;

public class EncountTableTests
{
    private readonly string _tableFile;

    public EncountTableTests()
    {
        var p5r_tblDir = Path.Join(".", "Data", "Tables", "P5R");
        
        _tableFile = Path.Join(p5r_tblDir, "ENCOUNT.TBL");
    }

    [Fact]
    public void EncountTable_LoadsFile()
    {
        var tbl = new EncountTable(_tableFile);
        
        Assert.Equal(1000, tbl.EnemyEncountersSegment.Count);
        Assert.Equal(1000, tbl.ForcedPartiesSegment.Count);
        Assert.Equal(16, tbl.ChallengeBattlesSegment.Count);
    }
}