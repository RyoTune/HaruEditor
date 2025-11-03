namespace HaruEditor.Core.Tables.Common;

public interface IReadWrite
{
    void Read(BinaryReader reader);
    void Write(BinaryWriter writer);
}