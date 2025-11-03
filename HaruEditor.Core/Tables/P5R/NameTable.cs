using Amicitia.IO.Binary;
using AtlusScriptLibrary.Common.Text.Encodings;
using HaruEditor.Core.Tables.Common;

namespace HaruEditor.Core.Tables.P5R;

public interface INameTable : IReadWrite
{
    string? GetName(NameType type, int id);
    void SetName(NameType type, int id, string value);
}

public class NameTableProxy : INameTable
{
    public NameTable? NameTable { get; set; }
    
    public string? GetName(NameType type, int id) => NameTable?.GetName(type, id);

    public void SetName(NameType type, int id, string value) => NameTable?.SetName(type, id, value);
    
    public void Read(BinaryReader reader) => NameTable?.Read(reader);

    public void Write(BinaryWriter writer) => NameTable?.Write(writer);
}

public class NameTable : INameTable
{
    private readonly Dictionary<NameType, List<string>> _nameSections = [];

    public NameTable(string nameTblFile) => LoadTable(nameTblFile);

    public string? GetName(NameType type, int id) => _nameSections[type][id];

    public void SetName(NameType type, int id, string value) => _nameSections[type][id] = value;

    private void LoadTable(string tblFile)
    {
        using var nameTbl = new BinaryObjectReader(tblFile, Endianness.Big, AtlusEncoding.Persona5RoyalEFIGS);
        const int numSections = 38; // 38 for P5R, 34 for P5
        
        for (var i = 0; i < numSections / 2; i++)
        {
            var type = (NameType)i;
            var strs = new List<string>();
            var strsPtrs = new List<ushort>();

            var size = nameTbl.ReadUInt32();
            var numPtrs = size / 2;
            for (var j = 0; j < numPtrs; j++)
            {
                strsPtrs.Add(nameTbl.ReadUInt16());
            }
            
            var targetPadding = (int)((0x10 - nameTbl.Position % 0x10) % 0x10);
            if (targetPadding > 0)
            {
                nameTbl.Seek(targetPadding, SeekOrigin.Current);
            }
            
            var basePos = nameTbl.Position;
            for (var j = 0; j < numPtrs; j++)
            {
                nameTbl.Seek(basePos + strsPtrs[j] + 4, SeekOrigin.Begin);

                var targetStr = nameTbl.ReadString(StringBinaryFormat.NullTerminated);

                if ((byte)targetStr[^1] == 10)
                {
                    targetStr = targetStr.Remove(targetStr.Length - 1, 1);
                }
                
                strs.Add(targetStr);
            }
            
            targetPadding = (int)((0x10 - nameTbl.Position % 0x10) % 0x10);
            if (targetPadding > 0)
            {
                nameTbl.Seek(targetPadding, SeekOrigin.Current);
            }

            _nameSections[type] = strs;
        }
    }

    public void Read(BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public enum NameType
{
    Arcana,
    Skill,
    SkillsAgain,
    Enemy,
    Persona,
    Trait,
    Accessory,
    Armor,
    Consumable,
    KeyItem,
    Material,
    MeleeWeapon,
    BattleAction,
    Outfit,
    SkillCard,
    PartyFirst,
    PartyLast,
    Confidant,
    RangedWeapon,
    _39
}