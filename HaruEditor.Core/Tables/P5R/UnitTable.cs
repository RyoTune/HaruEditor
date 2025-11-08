using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;

namespace HaruEditor.Core.Tables.P5R;

public class UnitTable : IReadWrite
{
    public UnitTable(INameTable nameTable, string file) : this(nameTable, File.OpenRead(file), true) {}

    public UnitTable(INameTable nameTable, Stream stream, bool ownsStream)
    {
        EnemyUnitStatsSegment = new(nameTable);
        EnemyElementalAffinitiesSegment = new(nameTable);
        PersonaElementalAffinitiesSegment = new(nameTable);
        UnitVoiceIndexSegment = new(nameTable);
        UnitVisualIndexSegment = new(nameTable);
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public EnemyUnitStatsSegment EnemyUnitStatsSegment { get; set; }
    public EnemyElementalAffinitiesSegment EnemyElementalAffinitiesSegment { get; set; }
    public PersonaElementalAffinitiesSegment PersonaElementalAffinitiesSegment { get; set; }
    public UnitVoiceIndexSegment UnitVoiceIndexSegment { get; set; }
    public UnitVisualIndexSegment UnitVisualIndexSegment { get; set; }
    
    [Browsable(false)]
    public UnknownSegment UnknownSegment { get; set; } = [];
    
    public void Read(BinaryReader reader)
    {
        EnemyUnitStatsSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        EnemyElementalAffinitiesSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        PersonaElementalAffinitiesSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        UnitVoiceIndexSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        UnitVisualIndexSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        UnknownSegment.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter writer)
    {
        EnemyUnitStatsSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        EnemyElementalAffinitiesSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        PersonaElementalAffinitiesSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        UnitVoiceIndexSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        UnitVisualIndexSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        UnknownSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        writer.BaseStream.SetLength(writer.BaseStream.Position);
    }
}

public class EnemyUnitStatsSegment(INameTable nameTable) : BaseSegment<EnemyUnitStats>(nameTable)
{
    public override uint ItemSize { get; } = 0x44;
}

public class EnemyElementalAffinitiesSegment(INameTable nameTable) : BaseSegment<ElementalAffinitiesEnemy>(nameTable)
{
    public override uint ItemSize { get; } = 0x28;
}

public class PersonaElementalAffinitiesSegment(INameTable nameTable) : BaseSegment<ElementalAffinitiesPersona>(nameTable)
{
    public override uint ItemSize { get; } = 0x28;
}

public class UnitVoiceIndexSegment(INameTable nameTable) : BaseSegment<UnitVoiceIndex>(nameTable)
{
    public override uint ItemSize { get; } = 0x18;
}

public class UnitVisualIndexSegment(INameTable nameTable) : BaseSegment<UnitVisualIndex>(nameTable)
{
    public override uint ItemSize { get; } = 0x6;
}

public class UnitVisualIndex(INameTable nameTable, int id) : IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Enemy, id);
        set => nameTable.SetName(NameType.Enemy, id, value ?? string.Empty);
    }
    
    public ushort PersonaIndex { get; set; }  // Persona ID when captured
    public ushort ModelIndex { get; set; }    // Model ID
    public ushort UnknownR { get; set; }      // Unknown / placeholder

    public void Read(BinaryReader reader)
    {
        PersonaIndex = reader.ReadUInt16();
        ModelIndex = reader.ReadUInt16();
        UnknownR = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(PersonaIndex);
        writer.Write(ModelIndex);
        writer.Write(UnknownR);
    }
}

public enum VoiceId : byte
{
    Jigaku,
    YoungMen,
    Priest,
    Oyaji,
    Majo,
    Lady,
    YoungWomen,
    Baba,
    Child,
    Kemono,
    Heehaw,
}

public enum TalkPerson : byte
{
    Timid,
    Irritable,
    Upbeat,
    Gloomy,
    Unknown4,
    Unknown5,
    Unknown6,
    Unknown7,
}

public class UnitVoiceIndex(INameTable nameTable, int id) : IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Enemy, id);
        set => nameTable.SetName(NameType.Enemy, id, value ?? string.Empty);
    }
    
    public VoiceId VoiceId { get; set; }           // Subtract 1 to get voicepack index
    public TalkPerson TalkPerson { get; set; }
    public byte VoiceABCValue { get; set; }
    public byte Padding { get; set; }

    public ushort TalkMoneyMin { get; set; }
    public ushort TalkMoneyMax { get; set; }

    public ushort[] TalkItem { get; set; }      // 4 entries
    public ushort[] TalkItemRare { get; set; }  // 4 entries

    public void Read(BinaryReader reader)
    {
        VoiceId = (VoiceId)reader.ReadByte();
        TalkPerson = (TalkPerson)reader.ReadByte();
        VoiceABCValue = reader.ReadByte();
        Padding = reader.ReadByte();

        TalkMoneyMin = reader.ReadUInt16();
        TalkMoneyMax = reader.ReadUInt16();

        TalkItem = new ushort[4];
        for (int i = 0; i < 4; i++)
            TalkItem[i] = reader.ReadUInt16();

        TalkItemRare = new ushort[4];
        for (int i = 0; i < 4; i++)
            TalkItemRare[i] = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)VoiceId);
        writer.Write((byte)TalkPerson);
        writer.Write(VoiceABCValue);
        writer.Write(Padding);

        writer.Write(TalkMoneyMin);
        writer.Write(TalkMoneyMax);

        for (var i = 0; i < 4; i++)
            writer.Write(TalkItem[i]);

        for (var i = 0; i < 4; i++)
            writer.Write(TalkItemRare[i]);
    }
}

public class ElementalAffinitiesEnemy(INameTable nameTable, int id) : ElementalAffinities, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Enemy, id);
        set => nameTable.SetName(NameType.Enemy, id, value ?? string.Empty);
    }
}

public class ElementalAffinitiesPersona(INameTable nameTable, int id) : ElementalAffinities, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Persona, id);
        set => nameTable.SetName(NameType.Persona, id, value ?? string.Empty);
    }
}

public class ElementalAffinities : IReadWrite
{
    public AffinityBitfield PhysAffinity { get; set; }
    public AffinityBitfield GunAffinity { get; set; }
    public AffinityBitfield FireAffinity { get; set; }
    public AffinityBitfield IceAffinity { get; set; }
    public AffinityBitfield ElecAffinity { get; set; }
    public AffinityBitfield WindAffinity { get; set; }
    public AffinityBitfield PsyAffinity { get; set; }
    public AffinityBitfield NukeAffinity { get; set; }
    public AffinityBitfield BlessAffinity { get; set; }
    public AffinityBitfield CurseAffinity { get; set; }
    public AffinityBitfield AlmightyAffinity { get; set; }
    public AffinityBitfield DizzyAffinity { get; set; }
    public AffinityBitfield ConfuseAffinity { get; set; }
    public AffinityBitfield FearAffinity { get; set; }
    public AffinityBitfield ForgetAffinity { get; set; }
    public AffinityBitfield HungerAffinity { get; set; }
    public AffinityBitfield SleepAffinity { get; set; }
    public AffinityBitfield RageAffinity { get; set; }
    public AffinityBitfield DespairAffinity { get; set; }
    public AffinityBitfield BrainwashAffinity { get; set; }

    public void Read(BinaryReader reader)
    {
        PhysAffinity      = new(reader);
        GunAffinity       = new(reader);
        FireAffinity      = new(reader);
        IceAffinity       = new(reader);
        ElecAffinity      = new(reader);
        WindAffinity      = new(reader);
        PsyAffinity       = new(reader);
        NukeAffinity      = new(reader);
        BlessAffinity     = new(reader);
        CurseAffinity     = new(reader);
        AlmightyAffinity  = new(reader);
        DizzyAffinity     = new(reader);
        ConfuseAffinity   = new(reader);
        FearAffinity      = new(reader);
        ForgetAffinity    = new(reader);
        HungerAffinity    = new(reader);
        SleepAffinity     = new(reader);
        RageAffinity      = new(reader);
        DespairAffinity   = new(reader);
        BrainwashAffinity = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        PhysAffinity.Write(writer);
        GunAffinity.Write(writer);
        FireAffinity.Write(writer);
        IceAffinity.Write(writer);
        ElecAffinity.Write(writer);
        WindAffinity.Write(writer);
        PsyAffinity.Write(writer);
        NukeAffinity.Write(writer);
        BlessAffinity.Write(writer);
        CurseAffinity.Write(writer);
        AlmightyAffinity.Write(writer);
        DizzyAffinity.Write(writer);
        ConfuseAffinity.Write(writer);
        FearAffinity.Write(writer);
        ForgetAffinity.Write(writer);
        HungerAffinity.Write(writer);
        SleepAffinity.Write(writer);
        RageAffinity.Write(writer);
        DespairAffinity.Write(writer);
        BrainwashAffinity.Write(writer);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class AffinityBitfield
{
    public AffinityFlags Flags { get; set; }
    public byte Multiplier { get; set; }

    public AffinityBitfield() { }

    public AffinityBitfield(BinaryReader reader)
    {
        Flags = (AffinityFlags)reader.ReadByte();
        Multiplier = reader.ReadByte();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)Flags);
        writer.Write(Multiplier);
    }
}

[Flags]
public enum AffinityFlags : byte
{
    Block               = 1 << 0, // was bit 7
    Repel               = 1 << 1,
    Drain               = 1 << 2,
    Weak                = 1 << 3,
    Resist              = 1 << 4,
    AilmentImmune       = 1 << 5,
    GuaranteeAilment    = 1 << 6,
    DoubleAilmentChance = 1 << 7  // was bit 0
}

public class EnemyUnitStats(INameTable nameTable, int id) : IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Enemy, id);
        set => nameTable.SetName(NameType.Enemy, id, value ?? string.Empty);
    }

    public UnitFlags Flags { get; set; }               // Flags
    public ArcanaID Arcana { get; set; }              // Arcana
    public byte RESERVE_05 { get; set; }              // RESERVE
    public ushort Level { get; set; }                 // Level
    public uint Hp { get; set; }                      // HP
    public uint Sp { get; set; }                      // SP
    public Stats Stats { get; set; }           // Enemy Stats
    public byte RESERVE_STAT { get; set; }            // RESERVE
    public BattleSkill[] SkillIds { get; set; }       // Battle Skills (8)
    public ushort ExpReward { get; set; }             // EXP Reward
    public ushort MoneyReward { get; set; }           // Money Reward
    public datEnemyItemTable[] DropTables { get; set; }   // Item Drop List (4)
    public datEnemyEventItemTable EventDrop { get; set; } // Event Item Drop
    public datEnemyAttackTable AttackDamage { get; set; } // Attack Attributes

    public void Read(BinaryReader reader)
    {
        Flags = (UnitFlags)reader.ReadUInt32();
        Arcana = (ArcanaID)reader.ReadByte();
        RESERVE_05 = reader.ReadByte();
        Level = reader.ReadUInt16();
        Hp = reader.ReadUInt32();
        Sp = reader.ReadUInt32();

        Stats = new(reader);

        RESERVE_STAT = reader.ReadByte();

        SkillIds = new BattleSkill[8];
        for (int i = 0; i < 8; i++)
            SkillIds[i] = (BattleSkill)reader.ReadUInt16();

        ExpReward = reader.ReadUInt16();
        MoneyReward = reader.ReadUInt16();

        DropTables = new datEnemyItemTable[4];
        for (int i = 0; i < 4; i++)
            DropTables[i] = new(reader);

        EventDrop = new(reader);
        AttackDamage = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)Flags);
        writer.Write((byte)Arcana);
        writer.Write(RESERVE_05);
        writer.Write(Level);
        writer.Write(Hp);
        writer.Write(Sp);

        Stats.Write(writer);
        writer.Write(RESERVE_STAT);

        for (int i = 0; i < 8; i++)
            writer.Write((ushort)SkillIds[i]);

        writer.Write(ExpReward);
        writer.Write(MoneyReward);

        for (int i = 0; i < 4; i++)
            WriteEnemyItem(writer, DropTables[i]);

        WriteEventDrop(writer, EventDrop);
        WriteAttack(writer, AttackDamage);
    }

    private static void WriteEnemyItem(BinaryWriter writer, datEnemyItemTable table)
    {
        writer.Write(table.ItemId);
        writer.Write(table.DropProbability);
    }

    private static void WriteEventDrop(BinaryWriter writer, datEnemyEventItemTable evt)
    {
        writer.Write(evt.EventID);
        writer.Write(evt.ItemId);
        writer.Write(evt.DropProbability);
    }

    private static void WriteAttack(BinaryWriter writer, datEnemyAttackTable atk)
    {
        writer.Write((byte)atk.Attribute);
        writer.Write(atk.Accuracy);
        writer.Write(atk.Damage);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class datEnemyItemTable
{
    public ushort ItemId { get; set; }           // Item ID
    public ushort DropProbability { get; set; }  // Drop Probability %

    public datEnemyItemTable() { }

    public datEnemyItemTable(BinaryReader reader)
    {
        ItemId = reader.ReadUInt16();
        DropProbability = reader.ReadUInt16();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class datEnemyEventItemTable
{
    public ushort EventID { get; set; }          // Event ID
    public ushort ItemId { get; set; }           // Item ID
    public ushort DropProbability { get; set; }  // Drop Probability %

    public datEnemyEventItemTable() { }

    public datEnemyEventItemTable(BinaryReader reader)
    {
        EventID = reader.ReadUInt16();
        ItemId = reader.ReadUInt16();
        DropProbability = reader.ReadUInt16();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class datEnemyAttackTable
{
    public ElementalType Attribute { get; set; } // Elemental Attribute
    public byte Accuracy { get; set; }           // Attack Accuracy
    public ushort Damage { get; set; }           // Attack Damage

    public datEnemyAttackTable() { }

    public datEnemyAttackTable(BinaryReader reader)
    {
        Attribute = (ElementalType)reader.ReadByte();
        Accuracy = reader.ReadByte();
        Damage = reader.ReadUInt16();
    }
}

[Flags]
public enum UnitFlags : uint
{
    Bit31 = 1 << 0,
    InfiniteSP = 1 << 1, // Enemies can use skills regardless of SP pool
    HidingStatus2 = 1 << 2, // Hidden status for bosses
    Bit28 = 1 << 3,
    Bit27 = 1 << 4,
    Bit26 = 1 << 5,
    Bit25 = 1 << 6,
    Bit24 = 1 << 7,
    Bit23 = 1 << 8,
    Bit22 = 1 << 9,
    NonNegotiable = 1 << 10, // Never negotiable
    GuaranteePersonaMask = 1 << 11, // Shadow always drops coin/persona
    Bit19 = 1 << 12,
    Bit18 = 1 << 13,
    HidingStatus = 1 << 14, // Hidden status like bosses
    NonBegging = 1 << 15, // Never beg regardless of personality
    Bit15 = 1 << 16,
    Bit14 = 1 << 17,
    Bit13 = 1 << 18,
    Bit12 = 1 << 19,
    Bit11 = 1 << 20,
    Bit10 = 1 << 21,
    Bit9 = 1 << 22,
    Bit8 = 1 << 23,
    Bit7 = 1 << 24,
    Bit6 = 1 << 25,
    Bit5 = 1 << 26,
    Bit4 = 1 << 27,
    Bit3 = 1 << 28,
    Bit2 = 1 << 29,
    Bit1 = 1 << 30,
    Bit0 = 1u << 31
}