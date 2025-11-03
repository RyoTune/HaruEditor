namespace HaruEditor.Core.Tables.Common;

public class UnknownSegment : BaseSegment<UnknownSegment>
{
    private byte[] _data = [];
    
    public override uint ItemSize { get; } = 0;

    public override void Read(BinaryReader reader)
    {
        var size = reader.ReadUInt32();
        _data = new byte[size];
        reader.BaseStream.ReadExactly(_data);
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(_data.Length);
        writer.BaseStream.Write(_data);
    }
}
