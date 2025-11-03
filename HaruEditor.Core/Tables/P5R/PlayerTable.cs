using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;

namespace HaruEditor.Core.Tables.P5R;

public class PlayerTable : IReadWrite
{
    public PlayerTable() {}

    public PlayerTable(string file) : this(File.OpenRead(file), true) {}

    public PlayerTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public PlayerLevelUpThresholdsSegment PlayerLevelUpThresholdsSegment { get; set; } = [];

    public PlayerHPSPPerLevelsSegment PlayerHpspPerLevelsSegment { get; set; } = [];

    [Browsable(false)]
    public UnknownSegment[] UnknownSegments { get; set; } = [[], [], [], []];
    
    public void Read(BinaryReader reader)
    {
        PlayerLevelUpThresholdsSegment.Read(reader);
        reader.BaseStream.AlignStream();

        PlayerHpspPerLevelsSegment.Read(reader);
        reader.BaseStream.AlignStream();

        foreach (var unkSeg in UnknownSegments)
        {
            unkSeg.Read(reader);
            reader.BaseStream.AlignStream();
        }
    }

    public void Write(BinaryWriter writer)
    {
        PlayerLevelUpThresholdsSegment.Write(writer);
        writer.BaseStream.AlignStream();

        PlayerHpspPerLevelsSegment.Write(writer);
        writer.BaseStream.AlignStream();

        foreach (var unkSeg in UnknownSegments)
        {
            unkSeg.Write(writer);
            writer.BaseStream.AlignStream();
        }
        
        writer.BaseStream.SetLength(writer.BaseStream.Position);
    }
}

public class PlayerLevelUpThresholdsSegment : BaseSegment<PlayerLevelUpThreshold>
{
    public override uint ItemSize { get; } = 0x4;
}

public class PlayerHPSPPerLevelsSegment : BaseSegment<PlayerHPSPPerLevels>
{
    public override uint ItemSize { get; } = 0x2C;
}

public class PlayerLevelUpThreshold : IReadWrite
{
    public PlayerLevelUpThreshold() {}

    public int Threshold { get; set; }
    
    public void Read(BinaryReader reader)
    {
        Threshold = reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(Threshold);
    }
}

public class PlayerHPSPPerLevels : IReadWrite
{
    public PlayerHPSPPerLevels() {}

    public HPSPEntry HPSPNone { get; set; }
    public HPSPEntry HPSPJoker { get; set; }
    public HPSPEntry HPSPRyuji { get; set; }
    public HPSPEntry HPSPMorgana { get; set; }
    public HPSPEntry HPSPAnn { get; set; }
    public HPSPEntry HPSPYusuke { get; set; }
    public HPSPEntry HPSPMakoto { get; set; }
    public HPSPEntry HPSPHaru { get; set; }
    public HPSPEntry HPSPFutaba { get; set; }
    public HPSPEntry HPSPAkechi { get; set; }
    public HPSPEntry HPSPKasumi { get; set; }

    public void Read(BinaryReader reader)
    {
        HPSPNone = new(reader);
        HPSPJoker = new(reader);
        HPSPRyuji = new(reader);
        HPSPMorgana = new(reader);
        HPSPAnn = new(reader);
        HPSPYusuke = new(reader);
        HPSPMakoto = new(reader);
        HPSPHaru = new(reader);
        HPSPFutaba = new(reader);
        HPSPAkechi = new(reader);
        HPSPKasumi = new(reader);
    }

    public void Write(BinaryWriter writer)
    {
        HPSPNone.Write(writer);
        HPSPJoker.Write(writer);
        HPSPRyuji.Write(writer);
        HPSPMorgana.Write(writer);
        HPSPAnn.Write(writer);
        HPSPYusuke.Write(writer);
        HPSPMakoto.Write(writer);
        HPSPHaru.Write(writer);
        HPSPFutaba.Write(writer);
        HPSPAkechi.Write(writer);
        HPSPKasumi.Write(writer);
    }
    
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HPSPEntry
    {
        public ushort HP { get; set; }
        public ushort SP { get; set; }

        public HPSPEntry(BinaryReader reader)
        {
            HP = reader.ReadUInt16();
            SP = reader.ReadUInt16();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(HP);
            writer.Write(SP);
        }
    }
}