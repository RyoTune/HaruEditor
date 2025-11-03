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
    public SkillTable() {}

    public SkillTable(string skillFile) : this(File.OpenRead(skillFile), true) {}

    public SkillTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public SkillElements SkillElements { get; set; } = [];
    public ActiveSkills ActiveSkills { get; set; } = [];
    public TechnicalComboMaps TechnicalComboMaps { get; set; } = [];
    public Traits Traits { get; set; } = [];
    public void Read(BinaryReader reader)
    {
        SkillElements.Read(reader);
        reader.BaseStream.AlignStream();

        ActiveSkills.Read(reader);
        reader.BaseStream.AlignStream();

        TechnicalComboMaps.Read(reader);
        reader.BaseStream.AlignStream();

        Traits.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter writer)
    {
        SkillElements.Write(writer);
        writer.BaseStream.AlignStream();

        ActiveSkills.Write(writer);
        writer.BaseStream.AlignStream();

        TechnicalComboMaps.Write(writer);
        writer.BaseStream.AlignStream();

        Traits.Write(writer);
        writer.BaseStream.AlignStream();
    }
}

public class SkillElements : BaseSegment<SkillElement>
{
    public override uint ItemSize { get; } = 0x8;
}

public class ActiveSkills : BaseSegment<ActiveSkill>
{
    public override uint ItemSize { get; } = 0x30;
}

public class TechnicalComboMaps : BaseSegment<TechnicalComboMap>
{
    public override uint ItemSize { get; } = 0x28;
}

public class Traits : BaseSegment<Trait>
{
    public override uint ItemSize { get; } = 0x3C;
}

public partial class Trait : ReactiveObject, IReadWrite
{
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private ushort _ord;
    [Reactive] private ushort _field2;
    [Reactive] private int _effectRate;
    [Reactive] private int _unionEffect;
    [Reactive] private float _effectSize;
    [Reactive] private int[] _traitEx = new int[10];
    [Reactive] private TraitFlags _traitFlags;

    public Trait() {}

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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
}

[Flags]
public enum AilmentStatus : uint
{
    Burn        = 1 << 0,   // Bit 0
    Freeze      = 1 << 1,   // Bit 1
    Shock       = 1 << 2,   // Bit 2
    Dizzy       = 1 << 3,   // Bit 3
    Confuse     = 1 << 4,   // Bit 4
    Fear        = 1 << 5,   // Bit 5
    Forget      = 1 << 6,   // Bit 6
    Hunger      = 1 << 7,   // Bit 7
    Sleep       = 1 << 8,   // Bit 8
    Rage        = 1 << 9,   // Bit 9
    Despair     = 1 << 10,  // Bit 10
    Brainwash   = 1 << 11,  // Bit 11

    // "Other ailments" occupies bits 12–31
    OtherAilmentsMask = 0xFFFFF000  // 20 bits starting from bit 12
}

public partial class ActiveSkill : ReactiveObject, IReadWrite
{
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
    
    public ActiveSkill() {}

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
        throw new NotImplementedException();
    }
}

[Flags]
public enum ValidTargets : byte
{
    //None = 0,
    Enemies = 1 << 0,
    Allies = 1 << 1
}

[Flags]
public enum Effect1 : byte
{
    //None = 0,
    Counter = 1 << 0,
    Mouse = 1 << 1,
    Gluttony = 1 << 2,
    KnockDown = 1 << 3,
    Unconscious = 1 << 4,
    WeakToAllElements = 1 << 5,
    Jealousy = 1 << 6,
    Wrath = 1 << 7
}

[Flags]
public enum Effect2 : byte
{
    //None = 0,
    Lust = 1 << 0,
    Panic = 1 << 1,
    Berserk = 1 << 2,
    Desperation = 1 << 3,
    Brainwash = 1 << 4,
    Despair = 1 << 5,
    Rage = 1 << 6,
    Sleep = 1 << 7
}

[Flags]
public enum Effect3 : byte
{
    //None = 0,
    Hunger = 1 << 0,
    Forget = 1 << 1,
    Fear = 1 << 2,
    Confuse = 1 << 3,
    Dizzy = 1 << 4,
    Shock = 1 << 5,
    Freeze = 1 << 6,
    Burn = 1 << 7
}

[Flags]
public enum Effect4 : byte
{
    //None = 0,
    CoverPsy = 1 << 0,
    CoverNuke = 1 << 1,
    CoverWind = 1 << 2,
    CoverElec = 1 << 3,
    CoverIce = 1 << 4,
    CoverFire = 1 << 5,
    InstakillShield = 1 << 6,
    BreakMagicShield = 1 << 7
}

[Flags]
public enum Effect5 : byte
{
    //None = 0,
    BreakPhysicalShield = 1 << 0,
    AilmentSusceptibility = 1 << 1,
    NegatePsyResist = 1 << 2,
    NegateNukeResist = 1 << 3,
    NegateWindResist = 1 << 4,
    NegateElecResist = 1 << 5,
    NegateIceResist = 1 << 6,
    NegateFireResist = 1 << 7
}

[Flags]
public enum Effect6 : byte
{
    //None = 0,
    MagicShield = 1 << 0,
    PhysicalShield = 1 << 1,
    CritWayUp = 1 << 2,
    CritUp = 1 << 3,
    RemoveDebuffs = 1 << 4,
    RemoveBuffs = 1 << 5,
    Concentrate = 1 << 6,
    Charge = 1 << 7
}

[Flags]
public enum BuffDebuff : byte
{
    //None = 0,
    AccuracyDown = 1 << 0,
    AccuracyUp = 1 << 1,
    DefenseDown = 1 << 2,
    DefenseUp = 1 << 3,
    EvasionDown = 1 << 4,
    EvasionUp = 1 << 5,
    AttackDown = 1 << 6,
    AttackUp = 1 << 7
}

public partial class SkillElement : ReactiveObject, IReadWrite
{
    [Reactive] [property: IgnoreDataMember] private int _id;
    [Reactive] private ElementalType _elementalType;
    [Reactive] private Skill_PassiveOrActive _isActive;
    [Reactive] private bool _isInheritable;
    
    public SkillElement() {}

    public void Read(BinaryReader reader)
    {
        ElementalType = (ElementalType)reader.ReadByte();
        IsActive = (Skill_PassiveOrActive)reader.ReadByte();
        IsInheritable = reader.ReadBoolean();
        reader.BaseStream.Position += 5;
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}