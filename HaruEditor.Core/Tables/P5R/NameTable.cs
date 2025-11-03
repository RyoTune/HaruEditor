using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using AtlusScriptLibrary.Common.Text.Encodings;

namespace HaruEditor.Core.Tables.P5R;

public interface INameTable
{
    string? GetName(NameType type, int id);
    
    void SetName(NameType type, int id, string value);

    void Read(Stream stream);

    void Write(Stream stream);
}

public class NameTableProxy : INameTable
{
    public NameTable? NameTable { get; set; }
    
    public string? GetName(NameType type, int id) => NameTable?.GetName(type, id);

    public void SetName(NameType type, int id, string value) => NameTable?.SetName(type, id, value);
    
    public void Read(Stream stream) => NameTable?.Read(stream);

    public void Write(Stream stream) => NameTable?.Write(stream);
}

public class NameTable : INameTable
{
    private readonly Dictionary<NameType, List<string>> _nameSections = [];

    public NameTable(string nameTblFile) => Read(File.OpenRead(nameTblFile));

    public string GetName(NameType type, int id) => _nameSections[type][id];

    public void SetName(NameType type, int id, string value) => _nameSections[type][id] = value;

    public void Read(Stream stream)
    {
        using var nameTbl = new BinaryObjectReader(stream, StreamOwnership.Transfer, Endianness.Big,
            AtlusEncoding.Persona5RoyalEFIGS);
        
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

    public void Write(Stream stream)
    {
        using var nameTbl = new BinaryObjectWriter(stream, StreamOwnership.Transfer, Endianness.Big,
            AtlusEncoding.Persona5RoyalEFIGS);

        const int numSections = 38; // 38 for P5R, 34 for P5
        for (int i = 0; i < numSections / 2; i++)
        {
            var strs = _nameSections[(NameType)i];
            var ptrs = new List<long>();
            var fileSizePos = nameTbl.Position;
            nameTbl.WriteUInt32(0);
            var numPtrs = strs.Count;
            var strPtrsPos = nameTbl.Position;
            for (int j = 0; j < numPtrs; j++)
            {
                nameTbl.WriteUInt16(0);
            }
            
            var fileSize = (uint)(nameTbl.Position - fileSizePos) - 4;
            int targetPadding = (int)((0x10 - nameTbl.Position % 0x10) % 0x10);
            if (targetPadding > 0)
            {
                for (int j = 0; j < targetPadding; j++)
                {
                    nameTbl.WriteByte(0);
                }
            }

            var basePos = nameTbl.Position;
            nameTbl.Seek(fileSizePos, SeekOrigin.Begin);
            nameTbl.WriteUInt32(fileSize);
            nameTbl.Seek(basePos, SeekOrigin.Begin);

            fileSizePos = nameTbl.Position;
            
            nameTbl.WriteUInt32(0);
            for (int j = 0; j < numPtrs; j++)
            {
                ptrs.Add(nameTbl.Position - (fileSizePos + 4));
                nameTbl.WriteString(StringBinaryFormat.NullTerminated, strs[j]);
            }
            
            fileSize = (uint)(nameTbl.Position - fileSizePos) - 4;
            
            targetPadding = (int)((0x10 - nameTbl.Position % 0x10) % 0x10);
            if (targetPadding > 0)
            {
                for (int j = 0; j < targetPadding; j++)
                {
                    nameTbl.WriteByte(0);
                }
            }

            basePos = nameTbl.Position;
            
            nameTbl.Seek(fileSizePos, SeekOrigin.Begin);
            nameTbl.WriteUInt32(fileSize);
            
            nameTbl.Seek(strPtrsPos, SeekOrigin.Begin);
            for (int j = 0; j < numPtrs; j++)
            {
                nameTbl.WriteUInt16((ushort)ptrs[j]);
            }
            
            nameTbl.Seek(basePos, SeekOrigin.Begin);
        }
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
    Unk1,
    Unk2,
    Unk3,
    Unk4
}