namespace HaruEditor.Core.Tables.Common;

public abstract class BaseSegment<TItem> : List<TItem>, ITableSegment<TItem>, IReadWrite
    where TItem : IReadWrite, new()
{
    public abstract uint ItemSize { get; }
    
    public virtual void Read(BinaryReader reader)
    {
        var segSize = reader.ReadUInt32();
        var numItems = segSize / ItemSize;
        for (var i = 0; i < numItems; i++)
        {
            var instance = Activator.CreateInstance<TItem>();
            instance.Read(reader);
            Add(instance);
        }
    }

    public virtual void Write(BinaryWriter writer)
    {
        writer.Write((uint)(Count * ItemSize));
        foreach (var item in this) item.Write(writer);
    }
}