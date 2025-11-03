using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HaruEditor.Desktop.Localization;

namespace HaruEditor.Desktop.Models;

public class Table
{
    public Table(object table)
    {
        Name = ObjectLocalizer.GetValue(table);
        Sections = table.GetType().GetProperties()
            .Where(x =>
            {
                if (x.IsDefined(typeof(BrowsableAttribute), true))
                {
                    var browsableAttr = x.GetCustomAttribute<BrowsableAttribute>(true)!;
                    return browsableAttr.Browsable;
                }
                
                return true;
            })
            .Select(x => x.GetValue(table))
            .Select(x => new TableSection((IEnumerable<object>)x!)).ToList();
    }

    public string Name { get; }
    public List<TableSection> Sections { get; }
}