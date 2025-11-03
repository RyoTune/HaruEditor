using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;

namespace HaruEditor.Core.Tables.P5R;

public class VisualTable : IReadWrite
{
    public VisualTable() {}

    public VisualTable(string file) : this(File.OpenRead(file), true) {}

    public VisualTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public VisualEnemyVisualVariablesASegment VisualEnemyVisualVariablesASegment { get; set; } = [];
    public VisualPlayerVisualVariablesASegment VisualPlayerVisualVariablesASegment { get; set; } = [];
    public VisualPersonaVisualVariablesASegment VisualPersonaVisualVariablesASegment { get; set; } = [];
    
    [Browsable(false)]
    public UnknownSegment[] UnknownSegments { get; set; } = [[], [], []];

    public void Read(BinaryReader reader)
    {
        VisualEnemyVisualVariablesASegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        VisualPlayerVisualVariablesASegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        VisualPersonaVisualVariablesASegment.Read(reader);
        reader.BaseStream.AlignStream();

        foreach (var unkSeg in UnknownSegments)
        {
            unkSeg.Read(reader);
            reader.BaseStream.AlignStream();
        }
    }

    public void Write(BinaryWriter writer)
    {
        VisualEnemyVisualVariablesASegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        VisualPlayerVisualVariablesASegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        VisualPersonaVisualVariablesASegment.Write(writer);
        writer.BaseStream.AlignStream();

        foreach (var unkSeg in UnknownSegments)
        {
            unkSeg.Write(writer);
            writer.BaseStream.AlignStream();
        }
    }
}

public class VisualEnemyVisualVariablesASegment : BaseSegment<VisualEnemyVisualVariablesA>
{
    public override uint ItemSize { get; } = 0xC8;
}

public class VisualPlayerVisualVariablesASegment : BaseSegment<VisualPlayerVisualVariablesA>
{
    public override uint ItemSize { get; } = 0x194;
}

public class VisualPersonaVisualVariablesASegment : BaseSegment<VisualPersonaVisualVariablesA>
{
    public override uint ItemSize { get; } = 0x94;
}

public class VisualPersonaVisualVariablesA : IReadWrite
{
    public uint Flags { get; set; }

    public datCollisionTable NormalCylinder { get; set; }
    public datCollisionTable ButuriCylinder { get; set; } // Phys Attack Camera
    public datCollisionTable SkillCCylinder { get; set; } // Skill Camera 1
    public datCollisionTable SkillSCylinder { get; set; } // Skill Camera 2

    public OffsetPosition ButuriPosition { get; set; } // Attack Position Offset
    public OffsetPosition MagicPosition { get; set; } // Magic Cast Offset

    public short BattleModelScale { get; set; } // Model Scale % (Battle)

    public TVisual_AttackFrameDataPersona FrameData { get; set; }

    public byte Alpha { get; set; } // Model Alpha
    public byte AlignmentReserve { get; set; }

    public void Read(BinaryReader reader)
    {
        Flags = reader.ReadUInt32();
        NormalCylinder = new(reader);
        ButuriCylinder = new(reader);
        SkillCCylinder = new(reader);
        SkillSCylinder = new(reader);
        ButuriPosition = new(reader);
        MagicPosition = new(reader);
        BattleModelScale = reader.ReadInt16();
        FrameData = new(reader);
        Alpha = reader.ReadByte();
        AlignmentReserve = reader.ReadByte();
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class TVisual_AttackFrameDataPersona
{
    public TVisual_AttackFrameData_Struct PersonaNormalAttack { get; set; }
    public TVisual_AttackFrameData_Struct PersonaMagicAttack { get; set; }

    public TVisual_AttackFrameDataPersona() { }

    public TVisual_AttackFrameDataPersona(BinaryReader reader)
    {
        PersonaNormalAttack = new(reader);
        PersonaMagicAttack = new(reader);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class OffsetPosition
{
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }

    public OffsetPosition() { }

    public OffsetPosition(BinaryReader reader)
    {
        X = reader.ReadInt16();
        Y = reader.ReadInt16();
        Z = reader.ReadInt16();
    }
}

public class VisualPlayerVisualVariablesA : IReadWrite
{
    public datCollisionTable[] CollisionTable { get; set; } = new datCollisionTable[13]; // Camera Data

    public ushort ModelScale { get; set; }               // Model Scale %
    public ushort AilmentIndicatorSize { get; set; }    // Ailment VFX Scale %
    public ushort EffectScale { get; set; }             // Effect Scale %
    public ushort UnkScale { get; set; }                // Unknown Scale %

    public datMoveTable ForwardMove { get; set; }
    public datMoveTable BackwardMove { get; set; }

    public TVisual_AttackFrameData FrameData { get; set; }

    public void Read(BinaryReader reader)
    {
        for (var i = 0; i < 13; i++)
            CollisionTable[i] = new(reader);

        ModelScale = reader.ReadUInt16();
        AilmentIndicatorSize = reader.ReadUInt16();
        EffectScale = reader.ReadUInt16();
        UnkScale = reader.ReadUInt16();

        ForwardMove = new(reader);
        BackwardMove = new(reader);

        FrameData = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class TVisual_AttackFrameData
{
    public TVisual_AttackFrameData_Struct PlayerNormalAttack { get; set; }
    public TVisual_AttackFrameData_Struct PlayerCritAttack { get; set; }
    public TVisual_AttackFrameData_Struct PlayerMissedAttack { get; set; }
    public TVisual_AttackFrameData_Struct PlayerItemUse { get; set; }
    public TVisual_AttackFrameData_Struct PlayerMagicCast { get; set; }

    public TVisual_AttackFrameData() { }

    public TVisual_AttackFrameData(BinaryReader reader)
    {
        PlayerNormalAttack = new(reader);
        PlayerCritAttack = new(reader);
        PlayerMissedAttack = new(reader);
        PlayerItemUse = new(reader);
        PlayerMagicCast = new(reader);
    }
}

public class VisualEnemyVisualVariablesA : IReadWrite
{
    public uint Flags { get; set; }

    public datCollisionTable NormalCylinder { get; set; }
    public datCollisionTable DyingCylinder { get; set; }
    public datCollisionTable DownCylinder { get; set; }
    public datCollisionTable Skill1Cylinder { get; set; }
    public datCollisionTable Skill2Cylinder { get; set; }

    public ushort ModelScale { get; set; }               // Model Scale %
    public ushort AilmentIndicatorSize { get; set; }    // Ailment VFX Scale %
    public ushort EffectScale { get; set; }             // Effect Scale %
    public ushort UnkScale { get; set; }                // Unknown Scale %

    public datMoveTable ForwardMove { get; set; }
    public datMoveTable BackwardMove { get; set; }

    public TVisual_AttackFrameDataEnemy FrameData { get; set; }

    public void Read(BinaryReader reader)
    {
        Flags = reader.ReadUInt32();
        NormalCylinder = new(reader);
        DyingCylinder = new(reader);
        DownCylinder = new(reader);
        Skill1Cylinder = new(reader);
        Skill2Cylinder = new(reader);
        ModelScale = reader.ReadUInt16();
        AilmentIndicatorSize = reader.ReadUInt16();
        EffectScale = reader.ReadUInt16();
        UnkScale = reader.ReadUInt16();
        ForwardMove = new(reader);
        BackwardMove = new(reader);
        FrameData = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        throw new NotImplementedException();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CameraCenter
{
    public float X { get; set; }  // X Offset
    public float Y { get; set; }  // Y Offset
    public float Z { get; set; }  // Z Offset

    public CameraCenter() { }

    public CameraCenter(BinaryReader reader)
    {
        X = reader.ReadSingle();
        Y = reader.ReadSingle();
        Z = reader.ReadSingle();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class datCollisionTable
{
    public CameraCenter Center { get; set; }
    public float VirtualHeight { get; set; }   // Unit Height
    public float VirtualRadius { get; set; }   // Unit Radius

    public datCollisionTable() { }

    public datCollisionTable(BinaryReader reader)
    {
        Center = new(reader);
        VirtualHeight = reader.ReadSingle();
        VirtualRadius = reader.ReadSingle();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class datMoveTable
{
    public uint Flags { get; set; }
    public float Speed { get; set; }

    public datMoveTable() { }

    public datMoveTable(BinaryReader reader)
    {
        Flags = reader.ReadUInt32();
        Speed = reader.ReadSingle();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class TVisual_AttackFrameData_Struct
{
    public short[] AttackHitRegFrames { get; set; } = new short[8]; // Hit Register Frames
    public short AttackAnimationSpeed { get; set; }
    public short DistanceFromTarget { get; set; } // Collision Distance
    public short Stop { get; set; }               // Stop Frames
    public short Blend { get; set; }              // Interpolation Frames

    public TVisual_AttackFrameData_Struct() { }

    public TVisual_AttackFrameData_Struct(BinaryReader reader)
    {
        for (int i = 0; i < 8; i++)
            AttackHitRegFrames[i] = reader.ReadInt16();

        AttackAnimationSpeed = reader.ReadInt16();
        DistanceFromTarget = reader.ReadInt16();
        Stop = reader.ReadInt16();
        Blend = reader.ReadInt16();
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class TVisual_AttackFrameDataEnemy
{
    public TVisual_AttackFrameData_Struct EnemyNormalAttack { get; set; }
    public TVisual_AttackFrameData_Struct EnemyMagicCast { get; set; }
    public TVisual_AttackFrameData_Struct EnemyUnkAttack { get; set; }

    public TVisual_AttackFrameDataEnemy() { }

    public TVisual_AttackFrameDataEnemy(BinaryReader reader)
    {
        EnemyNormalAttack = new(reader);
        EnemyMagicCast = new(reader);
        EnemyUnkAttack = new(reader);
    }
}

