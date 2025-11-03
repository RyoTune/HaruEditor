using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Core.Tables.P5R;

public class PersonaTable : IReadWrite
{
    public PersonaTable() {}

    public PersonaTable(string file) : this(File.OpenRead(file), true) {}

    public PersonaTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public PersonaStatsSegment PersonaStatsSegment { get; set; } = [];
    public PersonaSkillsAndStatGrowthsSegment PersonaSkillsAndStatGrowthsSegment { get; set; } = [];
    public PersonaLevelUpThresholdsSegment PersonaLevelUpThresholdsSegment { get; set; } = [];
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
    }
}

public class PersonaStatsSegment : BaseSegment<PersonaStats>
{
    public override uint ItemSize { get; } = 0xE;
}

public class PersonaSkillsAndStatGrowthsSegment : BaseSegment<PersonaSkillsAndStatGrowth>
{
    public override uint ItemSize { get; } = 0x46;
}

public class PersonaLevelUpThresholdsSegment : BaseSegment<PersonaLevelUpThresholds>
{
    public override uint ItemSize { get; } = 0x188;
}

public class PersonaPartyPersonasSegment : BaseSegment<PersonaPartyPersonas>
{
    public override uint ItemSize { get; } = 0x26E;
}

public class PersonaPartyPersonas : IReadWrite
{
    public PartyMember _member; // "Character"
    public byte _levelCount; // "Levels Available"
    //public byte _; // padding or mystery byte

    public PersonaSkillEntry[] _personaSkill = new PersonaSkillEntry[32];
    public Stats[] _statGain = new Stats[98];

    public PersonaPartyPersonas()
    {
    }

    public void Read(BinaryReader reader)
    {
        _member = (PartyMember)reader.ReadUInt16();
        _levelCount = reader.ReadByte();
        reader.ReadByte();

        for (int i = 0; i < _personaSkill.Length; i++)
            _personaSkill[i] = new(reader);

        for (int i = 0; i < _statGain.Length; i++)
            _statGain[i] = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    public class PersonaSkillEntry
    {
        public byte _level; // "Level Learned"
        public LearnableFlags _flags; // "Learnability"
        public PersonaSkillData _data;

        public PersonaSkillEntry(BinaryReader br)
        {
            _level = br.ReadByte();
            _flags = (LearnableFlags)br.ReadByte();
            _data = new(br);
        }
    }
}

public partial class PersonaLevelUpThresholds : ReactiveObject, IReadWrite
{
    [Reactive] private int[] _expThresholds = new int[98];

    public PersonaLevelUpThresholds()
    {
    }

    public void Read(BinaryReader reader)
    {
        for (int i = 0; i < ExpThresholds.Length; i++)
        {
            ExpThresholds[i] = reader.ReadInt32();
        }
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public partial class PersonaSkillsAndStatGrowth : ReactiveObject, IReadWrite
{
    [Reactive] private Stats _wStatDist;
    private byte _padding;
    [Reactive] private PersonaSkill[] _personaSkills = new PersonaSkill[16];

    public PersonaSkillsAndStatGrowth()
    {
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class PersonaSkill(BinaryReader br) : ReactiveObject
    {
        [Reactive] private byte _levelsDelta = br.ReadByte();
        [Reactive] private LearnableFlags _flags = (LearnableFlags)br.ReadByte();
        [Reactive] private PersonaSkillData _data = new(br);
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
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class PersonaSkillData : ReactiveObject
{
    [Reactive] private BattleSkill _skill;
    [Reactive] private BattleTrait _trait;
    [Reactive] private short _data;

    public PersonaSkillData(BinaryReader br)
    {
        Data = br.ReadInt16();
        Trait = (BattleTrait)Data;
        Skill = (BattleSkill)Data;
    }
}

public partial class PersonaStats : ReactiveObject, IReadWrite
{
    [Reactive] private PersonaFlags _flags;
    [Reactive] private ArcanaID _arcana;
    [Reactive] private byte _level;
    [Reactive] private Stats _stats;
    private byte _padding;                        
    [Reactive] private PersonaInherit _inherit;
    [Reactive] private ushort _unknown;

    public PersonaStats() {}

    public void Read(BinaryReader reader)
    {
        _flags = (PersonaFlags)reader.ReadUInt16();
        _arcana = (ArcanaID)reader.ReadByte();
        _level = reader.ReadByte();
        _stats = new(reader);
        _padding = reader.ReadByte();   // bitfield placeholder
        _inherit = (PersonaInherit)reader.ReadInt16();
        _unknown = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
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