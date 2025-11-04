using System.Collections.Generic;
using System.Linq;
using HaruEditor.Desktop.Localization;
using HaruEditor.Desktop.Project;

namespace HaruEditor.Desktop.Models;

public class TableSection
{
    public TableSection(ICommentService comments, IEnumerable<object> segment)
    {
        Name = ObjectLocalizer.GetValue(segment);
        Items = segment.Select((x, id) => new TableItem(comments, id, x)).ToList();
    }

    public string Name { get; }

    public List<TableItem> Items { get; }
}