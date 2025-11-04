using HaruEditor.Core.Tables.P5R;

namespace HaruEditor.Library.Tests;

public class SkillTableTests
{
    private readonly string _skillTableFile;

    public SkillTableTests()
    {
        var p5r_tblDir = Path.Join(".", "Data", "Tables", "P5R");
        
        _skillTableFile = Path.Join(p5r_tblDir, "SKILL.TBL");
    }

    [Fact]
    public void SkillTable_LoadsFile()
    {
        var skillTbl = new SkillTable(null, _skillTableFile);
        
        Assert.Equal(1056, skillTbl.SkillElementSegment.Count);
        Assert.Equal(800, skillTbl.ActiveSkillSegment.Count);
        Assert.Equal(17, skillTbl.TechnicalComboMapSegment.Count);
        Assert.Equal(299, skillTbl.TraitSegment.Count);
    }
}