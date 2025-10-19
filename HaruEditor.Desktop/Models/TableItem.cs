using System;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.Models;

public partial class TableItem(int id, object item) : ReactiveObject
{
    [Reactive] private ObjectWithId _item = new(id, item);
    [Reactive] private int _id = id;
    [Reactive] private string _tags = string.Empty;
    [Reactive] private string _comment = Guid.NewGuid().ToString();
}