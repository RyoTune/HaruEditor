using System.Runtime.Serialization;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

// TODO: Reverse all flag bit positions.
// BinaryReader converts BE values to LE, but flags still expect BE value.

// ReactiveUI.SourceGenerators attribute passthrough works despite the warning.
// Maybe not recognized as valid?
#pragma warning disable CS0657 // Not a valid attribute location for this declaration

namespace HaruEditor.Core.Tables.P5R;

public class SkillTable : IReadWrite
{
    public SkillTable(INameTable nameTable, string skillFile) : this(nameTable, File.OpenRead(skillFile), true) {}

    public SkillTable(INameTable nameTable, Stream stream, bool ownsStream)
    {
        SkillElementSegment = new(nameTable);
        ActiveSkillSegment = new(nameTable);
        TraitSegment = new(nameTable);
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public SkillElementSegment SkillElementSegment { get; set; }
    public ActiveSkillSegment ActiveSkillSegment { get; set; }
    public TechnicalComboMapSegment TechnicalComboMapSegment { get; set; } = [];
    public TraitSegment TraitSegment { get; set; }
    public void Read(BinaryReader reader)
    {
        SkillElementSegment.Read(reader);
        reader.BaseStream.AlignStream();

        ActiveSkillSegment.Read(reader);
        reader.BaseStream.AlignStream();

        TechnicalComboMapSegment.Read(reader);
        reader.BaseStream.AlignStream();

        TraitSegment.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter writer)
    {
        SkillElementSegment.Write(writer);
        writer.BaseStream.AlignStream();

        ActiveSkillSegment.Write(writer);
        writer.BaseStream.AlignStream();

        TechnicalComboMapSegment.Write(writer);
        writer.BaseStream.AlignStream();

        TraitSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        writer.BaseStream.SetLength(writer.BaseStream.Position);
    }
}

public class SkillElementSegment(INameTable nameTable) : BaseSegment<SkillElement>(nameTable)
{
    public override uint ItemSize { get; } = 0x8;
}

public class ActiveSkillSegment(INameTable nameTable) : BaseSegment<ActiveSkill>(nameTable)
{
    public override uint ItemSize { get; } = 0x30;
}

public class TechnicalComboMapSegment : BaseSegment<TechnicalComboMap>
{
    public override uint ItemSize { get; } = 0x28;
}

public class TraitSegment(INameTable nameTable) : BaseSegment<Trait>(nameTable)
{
    public override uint ItemSize { get; } = 0x3C;
}

public partial class Trait(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Trait, id);
        set => nameTable.SetName(NameType.Trait, id, value ?? string.Empty);
    }
    
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private ushort _ord;
    [Reactive] private ushort _field2;
    [Reactive] private int _effectRate;
    [Reactive] private int _unionEffect;
    [Reactive] private float _effectSize;
    [Reactive] private int[] _traitEx = new int[10];
    [Reactive] private TraitFlags _traitFlags;

    public void Read(BinaryReader reader)
    {
        Ord = reader.ReadUInt16();
        Field2 = reader.ReadUInt16();
        EffectRate = reader.ReadInt32();
        UnionEffect = reader.ReadInt32();
        EffectSize = reader.ReadSingle();

        for (var i = 0; i < TraitEx.Length; i++)
        {
            TraitEx[i] = reader.ReadInt32();
        }

        TraitFlags = (TraitFlags)reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_ord);
        writer.Write(_field2);
        writer.Write(_effectRate);
        writer.Write(_unionEffect);
        writer.Write(_effectSize);

        foreach (var ex in _traitEx)
            writer.Write(ex);

        writer.Write((int)_traitFlags);
    }
}

[Flags]
public enum TraitFlags : int
{
    None = 0,
    UseSubTrait = 1 << 0,
    IsTreasure = 1 << 1,
    IsUnique = 1 << 2
}

public partial class TechnicalComboMap : ReactiveObject, IReadWrite
{
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private AilmentStatus _applicableAilments;
    [Reactive] private uint _allAffinitiesAreTechnical;
    [Reactive] private Tech_TechnicalSkillAffinity[] _technicalSkillAffinity = new Tech_TechnicalSkillAffinity[5];
    [Reactive] private float _damageMultiplier;
    [Reactive] private uint _unknownR;
    [Reactive] private Tech_RequiresKnowingTheHeart _requiresKnowingTheHeart;
    
    public TechnicalComboMap() {}

    public void Read(BinaryReader reader)
    {
        ApplicableAilments = (AilmentStatus)reader.ReadUInt32();
        AllAffinitiesAreTechnical = reader.ReadUInt32();
        
        for (var i = 0; i < TechnicalSkillAffinity.Length; i++)
        {
            TechnicalSkillAffinity[i] = (Tech_TechnicalSkillAffinity)reader.ReadInt32();
        }

        DamageMultiplier = reader.ReadSingle();
        UnknownR = reader.ReadUInt32();
        RequiresKnowingTheHeart = (Tech_RequiresKnowingTheHeart)reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)_applicableAilments);
        writer.Write(_allAffinitiesAreTechnical);

        foreach (var tech in _technicalSkillAffinity)
            writer.Write((int)tech);

        writer.Write(_damageMultiplier);
        writer.Write(_unknownR);
        writer.Write((uint)_requiresKnowingTheHeart);
    }
}

public partial class ActiveSkill(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Skill, id);
        set => nameTable.SetName(NameType.Skill, id, value ?? string.Empty);
    }
    
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private byte _unknownR;
    [Reactive] private Skill_Condition _condition;
    [Reactive] private Skill_CasterEffect1 _casterEffect1;
    [Reactive] private Skill_CasterEffect2 _casterEffect2;
    [Reactive] private byte _unknownR2;
    [Reactive] private Skill_AreaType _placeUsage;
    [Reactive] private Skill_DamageStat _damageStat;
    [Reactive] private Skill_CostType _costType;
    [Reactive] private ushort _skillCost;
    [Reactive] private byte _add2;
    [Reactive] private Skill_PhysicalOrMagicSkill _physicalOrMagic;
    [Reactive] private Skill_NumberOfTargets _numberOfTargets;
    [Reactive] private ValidTargets _validTargets;
    [Reactive] private Skill_TargetRestrictions _targetRestrictions;
    [Reactive] private byte _unknown1;
    [Reactive] private byte _unknown2;
    [Reactive] private byte _unknown3;
    [Reactive] private byte _unknown4;
    [Reactive] private byte _unknown5;
    [Reactive] private byte _accuracy;
    [Reactive] private byte _minHits;
    [Reactive] private byte _maxHits;
    [Reactive] private Skill_HPEffect _hpEffect;
    [Reactive] private ushort _baseDamage;
    [Reactive] private Skill_SPEffect _spEffect;
    [Reactive] private byte _unknown6;
    [Reactive] private ushort _spAmount;
    [Reactive] private Skill_ApplyOrCureEffect _applyOrCureEffect;
    [Reactive] private byte _secondaryEffectChance;
    [Reactive] private byte _unknown7;
    [Reactive] private Effect1 _effect1;
    [Reactive] private Effect2 _effect2;
    [Reactive] private Effect3 _effect3;
    [Reactive] private Effect4 _effect4;
    [Reactive] private Effect5 _effect5;
    [Reactive] private Effect6 _effect6;
    [Reactive] private BuffDebuff _buffDebuff;
    [Reactive] private byte _unknownR3;
    [Reactive] private ushort _reserve2A;
    [Reactive] private Skill_OtherEffect _otherBuff;
    [Reactive] private Skill_ExtraEffect _extraEffect;
    [Reactive] private byte _critChance;
    [Reactive] private byte _forItem;
    [Reactive] private byte _unknown8;

    public void Read(BinaryReader reader)
    {
        UnknownR = reader.ReadByte();
        Condition = (Skill_Condition)reader.ReadByte();
        CasterEffect1 = (Skill_CasterEffect1)reader.ReadByte();
        CasterEffect2 = (Skill_CasterEffect2)reader.ReadByte();
        UnknownR2 = reader.ReadByte();
        PlaceUsage = (Skill_AreaType)reader.ReadByte();
        DamageStat = (Skill_DamageStat)reader.ReadByte();
        CostType = (Skill_CostType)reader.ReadByte();
        SkillCost = reader.ReadUInt16();
        Add2 = reader.ReadByte();
        PhysicalOrMagic = (Skill_PhysicalOrMagicSkill)reader.ReadByte();
        NumberOfTargets = (Skill_NumberOfTargets)reader.ReadByte();
        ValidTargets = (ValidTargets)reader.ReadByte();
        TargetRestrictions = (Skill_TargetRestrictions)reader.ReadByte();
        Unknown1 = reader.ReadByte();
        Unknown2 = reader.ReadByte();
        Unknown3 = reader.ReadByte();
        Unknown4 = reader.ReadByte();
        Unknown5 = reader.ReadByte();
        Accuracy = reader.ReadByte();
        MinHits = reader.ReadByte();
        MaxHits = reader.ReadByte();
        HpEffect = (Skill_HPEffect)reader.ReadByte();
        BaseDamage = reader.ReadUInt16();
        SpEffect = (Skill_SPEffect)reader.ReadByte();
        Unknown6 = reader.ReadByte();
        SpAmount = reader.ReadUInt16();
        ApplyOrCureEffect = (Skill_ApplyOrCureEffect)reader.ReadByte();
        SecondaryEffectChance = reader.ReadByte();
        Unknown7 = reader.ReadByte();
        Effect1 = (Effect1)reader.ReadByte();
        Effect2 = (Effect2)reader.ReadByte();
        Effect3 = (Effect3)reader.ReadByte();
        Effect4 = (Effect4)reader.ReadByte();
        Effect5 = (Effect5)reader.ReadByte();
        Effect6 = (Effect6)reader.ReadByte();
        BuffDebuff = (BuffDebuff)reader.ReadByte();
        UnknownR3 = reader.ReadByte();
        Reserve2A = reader.ReadUInt16();
        OtherBuff = (Skill_OtherEffect)reader.ReadByte();
        ExtraEffect = (Skill_ExtraEffect)reader.ReadByte();
        CritChance = reader.ReadByte();
        ForItem = reader.ReadByte();
        Unknown8 = reader.ReadByte();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_unknownR);
        writer.Write((byte)_condition);
        writer.Write((byte)_casterEffect1);
        writer.Write((byte)_casterEffect2);
        writer.Write(_unknownR2);
        writer.Write((byte)_placeUsage);
        writer.Write((byte)_damageStat);
        writer.Write((byte)_costType);
        writer.Write(_skillCost);
        writer.Write(_add2);
        writer.Write((byte)_physicalOrMagic);
        writer.Write((byte)_numberOfTargets);
        writer.Write((byte)_validTargets);
        writer.Write((byte)_targetRestrictions);
        writer.Write(_unknown1);
        writer.Write(_unknown2);
        writer.Write(_unknown3);
        writer.Write(_unknown4);
        writer.Write(_unknown5);
        writer.Write(_accuracy);
        writer.Write(_minHits);
        writer.Write(_maxHits);
        writer.Write((byte)_hpEffect);
        writer.Write(_baseDamage);
        writer.Write((byte)_spEffect);
        writer.Write(_unknown6);
        writer.Write(_spAmount);
        writer.Write((byte)_applyOrCureEffect);
        writer.Write(_secondaryEffectChance);
        writer.Write(_unknown7);
        writer.Write((byte)_effect1);
        writer.Write((byte)_effect2);
        writer.Write((byte)_effect3);
        writer.Write((byte)_effect4);
        writer.Write((byte)_effect5);
        writer.Write((byte)_effect6);
        writer.Write((byte)_buffDebuff);
        writer.Write(_unknownR3);
        writer.Write(_reserve2A);
        writer.Write((byte)_otherBuff);
        writer.Write((byte)_extraEffect);
        writer.Write(_critChance);
        writer.Write(_forItem);
        writer.Write(_unknown8);
    }
}

public enum ValidTargets : byte
{
    Allies = 1,
    Enemies,
    Any,
}

public partial class SkillElement(INameTable nameTable, int id) : ReactiveObject, IReadWrite, INameable
{
    public string? Name
    {
        get => nameTable.GetName(NameType.Skill, id);
        set => nameTable.SetName(NameType.Skill, id, value ?? string.Empty);
    }
    
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private ElementalType _elementalType;
    [Reactive] private Skill_PassiveOrActive _isActive;
    [Reactive] private byte _isInheritable;
    private readonly byte[] _unk1 = new byte[5];

    public void Read(BinaryReader reader)
    {
        ElementalType = (ElementalType)reader.ReadByte();
        IsActive = (Skill_PassiveOrActive)reader.ReadByte();
        IsInheritable = reader.ReadByte();
        reader.BaseStream.ReadExactly(_unk1);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)_elementalType);
        writer.Write((byte)_isActive);
        writer.Write(_isInheritable);
        writer.Write(_unk1);
    }
}

// Enum flags reversed below.
[Flags]
public enum Effect1 : byte
{
    Counter = 1 << 7,         // 0b10000000
    Mouse = 1 << 6,           // 0b01000000
    Gluttony = 1 << 5,        // 0b00100000
    KnockDown = 1 << 4,       // 0b00010000
    Unconscious = 1 << 3,     // 0b00001000
    WeakToAllElements = 1 << 2, // 0b00000100
    Jealousy = 1 << 1,        // 0b00000010
    Wrath = 1 << 0             // 0b00000001
}

[Flags]
public enum Effect2 : byte
{
    Lust = 1 << 7,
    Panic = 1 << 6,
    Berserk = 1 << 5,
    Desperation = 1 << 4,
    Brainwash = 1 << 3,
    Despair = 1 << 2,
    Rage = 1 << 1,
    Sleep = 1 << 0
}

[Flags]
public enum Effect3 : byte
{
    Hunger = 1 << 7,
    Forget = 1 << 6,
    Fear = 1 << 5,
    Confuse = 1 << 4,
    Dizzy = 1 << 3,
    Shock = 1 << 2,
    Freeze = 1 << 1,
    Burn = 1 << 0
}

[Flags]
public enum Effect4 : byte
{
    CoverPsy = 1 << 7,
    CoverNuke = 1 << 6,
    CoverWind = 1 << 5,
    CoverElec = 1 << 4,
    CoverIce = 1 << 3,
    CoverFire = 1 << 2,
    InstakillShield = 1 << 1,
    BreakMagicShield = 1 << 0
}

[Flags]
public enum Effect5 : byte
{
    BreakPhysicalShield = 1 << 7,
    AilmentSusceptibility = 1 << 6,
    NegatePsyResist = 1 << 5,
    NegateNukeResist = 1 << 4,
    NegateWindResist = 1 << 3,
    NegateElecResist = 1 << 2,
    NegateIceResist = 1 << 1,
    NegateFireResist = 1 << 0
}

[Flags]
public enum Effect6 : byte
{
    MagicShield = 1 << 7,
    PhysicalShield = 1 << 6,
    CritWayUp = 1 << 5,
    CritUp = 1 << 4,
    RemoveDebuffs = 1 << 3,
    RemoveBuffs = 1 << 2,
    Concentrate = 1 << 1,
    Charge = 1 << 0
}

[Flags]
public enum BuffDebuff : byte
{
    AccuracyDown = 1 << 7,
    AccuracyUp = 1 << 6,
    DefenseDown = 1 << 5,
    DefenseUp = 1 << 4,
    EvasionDown = 1 << 3,
    EvasionUp = 1 << 2,
    AttackDown = 1 << 1,
    AttackUp = 1 << 0
}

[Flags]
public enum AilmentStatus : uint
{
    Burn        = 1u << 0,  // AilmentStatus_00_Burn
    Freeze      = 1u << 1,  // AilmentStatus_01_Freeze
    Shock       = 1u << 2,  // AilmentStatus_02_Shock
    Dizzy       = 1u << 3,  // AilmentStatus_03_Dizzy
    Confuse     = 1u << 4,  // AilmentStatus_04_Confuse
    Fear        = 1u << 5,  // AilmentStatus_05_Fear
    Forget      = 1u << 6,  // AilmentStatus_06_Forget
    Hunger      = 1u << 7,  // AilmentStatus_07_Hunger
    Sleep       = 1u << 8,  // AilmentStatus_08_Sleep
    Rage        = 1u << 9,  // AilmentStatus_09_Rage
    Despair     = 1u << 10, // AilmentStatus_10_Despair
    Brainwash   = 1u << 11, // AilmentStatus_11_Brainwash

    // Bits 12–31 (20 bits total)
    //OtherAilmentsMask = 0xFFFFF000u
}