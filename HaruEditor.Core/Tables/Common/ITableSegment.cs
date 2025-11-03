namespace HaruEditor.Core.Tables.Common;

public interface ITableSegment<TItem> : IList<TItem>
    where TItem : IReadWrite, new()
{
    public uint ItemSize { get; }
}