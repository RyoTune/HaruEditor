using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using HaruEditor.Core.Tables.P5R;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [Reactive] private DataItem? _selectedItem;
    [Reactive] private int _selectedTab;
    
    public MainWindowViewModel()
    {
        SkillTable = new(Path.Join(".", "Tables", "SKILL.TBL"));

        this.WhenValueChanged(x => x.SelectedTab).Subscribe(_ => SelectedItem = null);
    }

    public SkillTable SkillTable { get; set; }

    public List<DataItem> ActiveSkillsList => SkillTable.ActiveSkills
        .Select(x => new DataItem(x, x.Id, Guid.NewGuid().ToString(), String.Empty, string.Empty)).ToList();

    public Interaction<object, Unit> EditWithPropertyGrid { get; } = new();

    [ReactiveCommand]
    private async Task EditItem(object? item)
    {
        if (item == null) return;
        await EditWithPropertyGrid.Handle(item);
    }
}

public partial class DataItem(object item, int id, string name, string tags, string comment) : ReactiveObject
{
    [Reactive] private object _item = item;
    [Reactive] private int _id = id;
    [Reactive] private string _name = name;
    [Reactive] private string _tags = tags;
    [Reactive] private string _comment = comment;
}