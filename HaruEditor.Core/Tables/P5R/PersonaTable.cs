using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Core.Tables.P5R;

public class PersonaTable : IReadWrite
{
    public PersonaTable(INameTable nameTable, string file) : this(nameTable, File.OpenRead(file), true) {}

    public PersonaTable(INameTable nameTable, Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        PersonaStatsSegment = new(nameTable);
        PersonaSkillsAndStatGrowthsSegment = new(nameTable);
        PersonaLevelUpThresholdsSegment = new(nameTable);
        Read(reader);
    }

    public PersonaStatsSegment PersonaStatsSegment { get; set; }
    public PersonaSkillsAndStatGrowthsSegment PersonaSkillsAndStatGrowthsSegment { get; set; }
    public PersonaLevelUpThresholdsSegment PersonaLevelUpThresholdsSegment { get; set; }
    public PersonaPartyPersonasSegment PersonaPartyPersonasSegment { get; set; } = [];
    
    public void Read(BinaryReader reader)
    {
        PersonaStatsSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        PersonaSkillsAndStatGrowthsSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        PersonaLevelUpThresholdsSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        PersonaPartyPersonasSegment.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter writer)
    {
        PersonaStatsSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        PersonaSkillsAndStatGrowthsSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        PersonaLevelUpThresholdsSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        PersonaPartyPersonasSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        writer.BaseStream.SetLength(writer.BaseStream.Position);
    }
}

public class PersonaStatsSegment(INameTable nameTable) : BaseSegment<PersonaStats>(nameTable)
{
    public override uint ItemSize { get; } = 0xE;
}

public class PersonaSkillsAndStatGrowthsSegment(INameTable nameTable) : BaseSegment<PersonaSkillsAndStatGrowth>(nameTable)
{
    public override uint ItemSize { get; } = 0x46;
}

public class PersonaLevelUpThresholdsSegment(INameTable nameTable) : BaseSegment<PersonaLevelUpThresholds>(nameTable)
{
    public override uint ItemSize { get; } = 0x188;
}

public class PersonaPartyPersonasSegment : BaseSegment<PersonaPartyPersonas>
{
    public override uint ItemSize { get; } = 0x26E;
}

public partial class PersonaPartyPersonas : ReactiveObject, IReadWrite
{
    [Reactive] public PartyMember _member;
    [Reactive] public byte _levelCount;
    public byte _unk1;

    [Reactive] public PersonaSkillEntry[] _personaSkill = new PersonaSkillEntry[32];
    [Reactive] public Stats[] _statGain = new Stats[98];

    public PersonaPartyPersonas()
    {
    }

    public void Read(BinaryReader reader)
    {
        _member = (PartyMember)reader.ReadUInt16();
        _levelCount = reader.ReadByte();
        _unk1 = reader.ReadByte();

        for (int i = 0; i < _personaSkill.Length; i++)
            _personaSkill[i] = new(reader);

        for (int i = 0; i < _statGain.Length; i++)
            _statGain[i] = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((ushort)_member);
        writer.Write(_levelCount);
        writer.Write(_unk1);

        for (int i = 0; i < _personaSkill.Length; i++)
            _personaSkill[i].Write(writer);

        for (int i = 0; i < _statGain.Length; i++)
            _statGain[i].Write(writer);
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class PersonaSkillEntry : ReactiveObject
    {
        [Reactive] public byte _level; // "Level Learned"
        [Reactive] public LearnableFlags _flags; // "Learnability"
        [Reactive] public PersonaSkillData _data;

        public PersonaSkillEntry(BinaryReader br)
        {
            _level = br.ReadByte();
            _flags = (LearnableFlags)br.ReadByte();
            _data = new(br);
        }
        
        public void Write(BinaryWriter writer)
        {
            writer.Write(_level);
            writer.Write((byte)_flags);
            _data.Write(writer);
        }
    }
}

public partial class PersonaLevelUpThresholds(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.PartyFirst, id);
        set => nameTable.SetName(NameType.PartyFirst, id, value ?? string.Empty);
    }
    
    [Reactive] private int[] _expThresholds = new int[98];

    public void Read(BinaryReader reader)
    {
        for (int i = 0; i < ExpThresholds.Length; i++)
        {
            ExpThresholds[i] = reader.ReadInt32();
        }
    }

    public void Write(BinaryWriter writer)
    {
        foreach (var exps in _expThresholds)
            writer.Write(exps);
    }
}

public partial class PersonaSkillsAndStatGrowth(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Persona, id);
        set => nameTable.SetName(NameType.Persona, id, value ?? string.Empty);
    }
    
    [Reactive] private Stats _wStatDist;
    private byte _padding;
    [Reactive] private PersonaSkill[] _personaSkills = new PersonaSkill[16];

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class PersonaSkill(BinaryReader br) : ReactiveObject
    {
        [Reactive] private byte _levelsDelta = br.ReadByte();
        [Reactive] private LearnableFlags _flags = (LearnableFlags)br.ReadByte();
        [Reactive] private PersonaSkillData _data = new(br);
        
        public void Write(BinaryWriter writer)
        {
            writer.Write(_levelsDelta);
            writer.Write((byte)_flags);
            _data.Write(writer);
        }
    }

    public void Read(BinaryReader reader)
    {
        _wStatDist = new(reader);
        _padding = reader.ReadByte();

        for (int i = 0; i < 16; i++)
            _personaSkills[i] = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        _wStatDist.Write(writer);
        writer.Write(_padding);
    
        for (int i = 0; i < _personaSkills.Length; i++)
            _personaSkills[i].Write(writer);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class PersonaSkillData(BinaryReader br) : ReactiveObject
{
    private short _data = br.ReadInt16();

    public BattleSkill Skill
    {
        get => (BattleSkill)Data;
        set => Data = (short)value;
    }

    public BattleTrait Trait
    {
        get => (BattleTrait)Data;
        set => Data = (short)value;
    }

    private short Data
    {
        get => _data;
        set
        {
            this.RaiseAndSetIfChanged(ref _data, value);
            this.RaisePropertyChanged(nameof(Trait));
            this.RaisePropertyChanged(nameof(Skill));
        }
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_data);
    }
}

public partial class PersonaStats(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Persona, id);
        set => nameTable.SetName(NameType.Persona, id, value ?? string.Empty);
    }
    
    [Reactive] private PersonaFlags _flags;
    [Reactive] private ArcanaID _arcana;
    [Reactive] private byte _level;
    [Reactive] private Stats _stats;
    private byte _padding;                        
    [Reactive] private PersonaInherit _inherit;
    [Reactive] private ushort _unknown;

    public void Read(BinaryReader reader)
    {
        _flags = (PersonaFlags)reader.ReadUInt16();
        _arcana = (ArcanaID)reader.ReadByte();
        _level = reader.ReadByte();
        _stats = new(reader);
        _padding = reader.ReadByte();
        _inherit = (PersonaInherit)reader.ReadInt16();
        _unknown = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((ushort)_flags);
        writer.Write((byte)_arcana);
        writer.Write(_level);
        _stats.Write(writer);
        writer.Write(_padding);
        writer.Write((short)_inherit);
        writer.Write(_unknown);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class Stats(BinaryReader br) : ReactiveObject
{
    [Reactive] private byte _strength = br.ReadByte();
    [Reactive] private byte _magic = br.ReadByte();
    [Reactive] private byte _endurance = br.ReadByte();
    [Reactive] private byte _agility = br.ReadByte();
    [Reactive] private byte _luck = br.ReadByte();
    
    public void Write(BinaryWriter writer)
    {
        writer.Write(_strength);
        writer.Write(_magic);
        writer.Write(_endurance);
        writer.Write(_agility);
        writer.Write(_luck);
    }
}

[Flags]
public enum PersonaFlags : ushort
{
    Evolved = 1 << 0,
    NoNormalFusion = 1 << 1,
    Unknown3 = 1 << 2,
    NotRegisterable = 1 << 3,
    StoryPersona = 1 << 4,
    PartyPersona = 1 << 5,
    Unknown2 = 1 << 6,
    Unknown1 = 1 << 7,
    Treasure = 1 << 8,
    Dlc = 1 << 9,
}