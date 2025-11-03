using HaruEditor.Core.Tables.P5R;

namespace HaruEditor.Core.Tables.Common;

public abstract class BaseSegment<TItem> : List<TItem>, ITableSegment<TItem>, IReadWrite
    where TItem : IReadWrite
{
    private readonly INameTable? _nameTable;

    protected BaseSegment()
    {
    }

    protected BaseSegment(INameTable nameTable)
    {
        _nameTable = nameTable;
    }
    
    public abstract uint ItemSize { get; }
    
    public virtual void Read(BinaryReader reader)
    {
        var segSize = reader.ReadUInt32();
        var numItems = segSize / ItemSize;
        var numParams = typeof(TItem).GetConstructors().First().GetParameters().Length;
        for (var i = 0; i < numItems; i++)
        {
            TItem instance;
            if (numParams == 0)
            {
                instance = Activator.CreateInstance<TItem>();
            }
            else
            {
                instance = (TItem)Activator.CreateInstance(typeof(TItem), _nameTable, i)!;
            }
            
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