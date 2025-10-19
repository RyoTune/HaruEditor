using System.Collections.Generic;
using System.Linq;
using HaruEditor.Desktop.Localization;

namespace HaruEditor.Desktop.Models;

public class TableSection
{
    public TableSection(IEnumerable<object> segment)
    {
        Name = ObjectLocalizer.GetValue(segment);
        Items = segment.Select((x, id) => new TableItem(id, x)).ToList();
    }

    public string Name { get; }

    public List<TableItem> Items { get; }
}