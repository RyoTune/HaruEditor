using HaruEditor.Core.Tables.P5R;

namespace HaruEditor.Library.Tests.Tables.P5R;

public class PersonaTableTests
{
    private readonly string _tableFile;

    public PersonaTableTests()
    {
        var p5r_tblDir = Path.Join(".", "Data", "Tables", "P5R");
        
        _tableFile = Path.Join(p5r_tblDir, "PERSONA.TBL");
    }

    [Fact]
    public void PersonaTable_LoadsFile()
    {
        var tbl = new PersonaTable(null, _tableFile);
        
        Assert.Equal(464, tbl.PersonaStatsSegment.Count);
        Assert.Equal(464, tbl.PersonaSkillsAndStatGrowthsSegment.Count);
    }
}