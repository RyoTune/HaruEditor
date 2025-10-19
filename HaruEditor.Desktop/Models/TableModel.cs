using System.Collections.Generic;
using System.Linq;
using HaruEditor.Desktop.Localization;

namespace HaruEditor.Desktop.Models;

public class TableModel
{
    public TableModel(object table)
    {
        Name = ObjectLocalizer.GetValue(table);
        Sections = table.GetType().GetProperties().Select(x => x.GetValue(table))
            .Select(x => new TableSection((IEnumerable<object>)x!)).ToList();
    }

    public string Name { get; }
    public List<TableSection> Sections { get; }
}