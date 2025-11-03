using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

#pragma warning disable CS0657 // Not a valid attribute location for this declaration

namespace HaruEditor.Core.Tables.P5R;

public class EncountTable : IReadWrite
{
    public EncountTable() {}

    public EncountTable(string file) : this(File.OpenRead(file), true) {}

    public EncountTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public EnemyEncountersSegment EnemyEncountersSegment { get; set; } = [];
    public ForcedPartiesSegment ForcedPartiesSegment { get; set; } = [];
    public ChallengeBattlesSegment ChallengeBattlesSegment { get; set; } = [];
    
    public void Read(BinaryReader reader)
    {
        EnemyEncountersSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ForcedPartiesSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ChallengeBattlesSegment.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter reader)
    {
        EnemyEncountersSegment.Write(reader);
        reader.BaseStream.AlignStream();
        
        ForcedPartiesSegment.Write(reader);
        reader.BaseStream.AlignStream();
        
        ChallengeBattlesSegment.Write(reader);
        reader.BaseStream.AlignStream();
    }
}

public class EnemyEncountersSegment : BaseSegment<EnemyEncounter>
{
    public override uint ItemSize { get; } = 0x2C;
}

public class ForcedPartiesSegment : BaseSegment<ForcedParty>
{
    public override uint ItemSize { get; } = 0x8;
}

public class ChallengeBattlesSegment : BaseSegment<ChallengeBattle>
{
    public override uint ItemSize { get; } = 0xA4;
}

public partial class ChallengeBattle : ReactiveObject, IReadWrite
{
    [Reactive] private ushort _category;
    [Reactive] private ushort _categoryIdx;
    [Reactive] private uint _flag;

    [Reactive] private ushort _turnBonusCount;
    [Reactive] private uint _turnBonus;

    [Reactive] private BonusEntry[] _bonus = new BonusEntry[5];
    [Reactive] private uint[] _waveEncounter = new uint[5];

    [Reactive] private uint _level;
    [Reactive] private uint _iconCount;

    [Reactive] private AwardEntry[] _award = new AwardEntry[3];

    public ChallengeBattle()
    {
    }

    public void Read(BinaryReader reader)
    {
        _category = reader.ReadUInt16();
        _categoryIdx = reader.ReadUInt16();
        reader.ReadUInt32(); // _1 (discarded)
        _flag = reader.ReadUInt32();

        _turnBonusCount = reader.ReadUInt16();
        reader.ReadUInt16(); // _2 (discarded)
        _turnBonus = reader.ReadUInt32();

        for (var i = 0; i < 5; i++)
            _bonus[i] = new(reader);

        for (var i = 0; i < 5; i++)
            _waveEncounter[i] = reader.ReadUInt32();

        for (var i = 0; i < 5; i++)
            reader.ReadUInt32(); // _3[i] (discarded)

        _level = reader.ReadUInt32();
        _iconCount = reader.ReadUInt32();

        for (var i = 0; i < 3; i++)
            _award[i] = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class BonusEntry : ReactiveObject
{
    [Reactive] private uint _target;
    [Reactive] private uint _type;
    [Reactive] private float _mult;

    public BonusEntry(BinaryReader br)
    {
        _target = br.ReadUInt32();
        _type = br.ReadUInt32();
        _mult = br.ReadSingle();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class AwardEntry : ReactiveObject
{
    [Reactive] private uint _requiredScore;
    [Reactive] private ushort _itemId;

    public AwardEntry(BinaryReader br)
    {
        _requiredScore = br.ReadUInt32();
        br.ReadUInt16(); // _4 (discarded)
        br.ReadUInt16(); // _5 (discarded)
        br.ReadUInt16(); // _6 (discarded)
        _itemId = br.ReadUInt16();
    }
}

public partial class ForcedParty : ReactiveObject, IReadWrite
{
    [Reactive] private PartyMember[] _playerID = new PartyMember[4];

    public ForcedParty()
    {
    }

    public void Read(BinaryReader reader)
    {
        for (var i = 0; i < _playerID.Length; i++)
        {
            _playerID[i] = (PartyMember)reader.ReadUInt16();
        }
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

public partial class EnemyEncounter : ReactiveObject, IReadWrite
{
    [Reactive] private BattleFlags _flags;
    [Reactive] private ushort _field04;
    [Reactive] private ushort _field06;
    [Reactive] private BattleUnit[] _units = new BattleUnit[5];
    [Reactive] private ushort _fieldId;
    [Reactive] private ushort _roomId;
    [Reactive] private MusicID _musicId;
    [Reactive] private TRoyalEncounter _extraData;

    public EnemyEncounter()
    {
    }

    public void Read(BinaryReader reader)
    {
        _flags = (BattleFlags)reader.ReadUInt32(); 
        _field04 = reader.ReadUInt16();
        _field06 = reader.ReadUInt16();

        // Read 5 BattleUnits
        for (var i = 0; i < _units.Length; i++)
        {
            _units[i] = (BattleUnit)reader.ReadInt16();
        }

        _fieldId = reader.ReadUInt16();
        _roomId = reader.ReadUInt16();
        _musicId = (MusicID)reader.ReadUInt16();

        _extraData = new()
        {
            EnemyReplacementData = new()
            {
                ReplacementEnemyID = reader.ReadUInt16(),
                OverallReplacementChance = reader.ReadByte(),
                Slot1Chance = reader.ReadByte(),
                Slot2Chance = reader.ReadByte(),
                Slot3Chance = reader.ReadByte(),
                Slot4Chance = reader.ReadByte(),
                Slot5Chance = reader.ReadByte()
            },
            DisasterData = new()
            {
                OverallDisasterChance = reader.ReadByte(),
                Slot1Chance = reader.ReadByte(),
                Slot2Chance = reader.ReadByte(),
                Slot3Chance = reader.ReadByte(),
                Slot4Chance = reader.ReadByte(),
                Slot5Chance = reader.ReadByte(),
                MaxDisasterShadows = reader.ReadByte()
            },
            Field0f = reader.ReadByte(),
            Field10 = reader.ReadByte(),
            Field11 = reader.ReadByte(),
            Field12 = reader.ReadByte(),
            Field13 = reader.ReadByte()
        };
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class TRoyalEncounter : ReactiveObject
{
    [Reactive] private TRoyalEnemyReplace _enemyReplacementData;
    [Reactive] private TRoyalDisasterReplace _disasterData;
    [Reactive] private byte _field0f;
    [Reactive] private byte _field10;
    [Reactive] private byte _field11;
    [Reactive] private byte _field12;
    [Reactive] private byte _field13;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class TRoyalEnemyReplace : ReactiveObject
{
    [Reactive] private ushort _replacementEnemyID;
    [Reactive] private byte _overallReplacementChance;
    [Reactive] private byte _slot1Chance;
    [Reactive] private byte _slot2Chance;
    [Reactive] private byte _slot3Chance;
    [Reactive] private byte _slot4Chance;
    [Reactive] private byte _slot5Chance;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public partial class TRoyalDisasterReplace : ReactiveObject
{
    [Reactive] private byte _overallDisasterChance;
    [Reactive] private byte _slot1Chance;
    [Reactive] private byte _slot2Chance;
    [Reactive] private byte _slot3Chance;
    [Reactive] private byte _slot4Chance;
    [Reactive] private byte _slot5Chance;
    [Reactive] private byte _maxDisasterShadows;
}

[Flags]
public enum BattleFlags : uint
{
    NoEscape = 1 << 0,
    Bit30 = 1 << 1,
    Bit29 = 1 << 2,
    NoCritical = 1 << 3,
    EnemyFirstAct = 1 << 4,
    LoadBattleScript2 = 1 << 5,
    Bit25 = 1 << 6,
    Bit24 = 1 << 7,
    LoadBattleScript = 1 << 8,
    Bit22 = 1 << 9,
    NoNavigator = 1 << 10,
    BulletHailOnStart = 1 << 11,
    ShadowNotDisappearInBattle = 1 << 12,
    NoHoldUp = 1 << 13,
    PositionHack = 1 << 14,
    PreventKnockdown = 1 << 15,
    Bit15 = 1 << 16,
    Bit14 = 1 << 17,
    Bit13 = 1 << 18,
    NoNegotiation = 1 << 19,
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

