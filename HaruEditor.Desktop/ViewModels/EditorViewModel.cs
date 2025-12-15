using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.P5R;
using HaruEditor.Desktop.Localization;
using HaruEditor.Desktop.Models;
using HaruEditor.Desktop.Project;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.ViewModels;

public partial class EditorViewModel : ViewModelBase, IActivatableViewModel
{
    private static readonly TimeSpan DefaultThrottle = TimeSpan.FromMilliseconds(300);
    
    public ViewModelActivator Activator { get; } = new();
    
    [Reactive] private TableItem? _selectedItem;
    [Reactive] private ObservableCollection<Table> _tables = [];
    [Reactive] private TableSection? _selectedSection;
    [Reactive] private SearchItem? _selectedSearchItem;
    [Reactive] private string? _searchText;
    [Reactive] private double? _jumpId;
    
    private readonly ProjectService _project;
    private readonly Dictionary<Table, string> _tableFileMap = [];
    private readonly NameTableProxy _nameTableProxy = new();
    private string? _nameTableFile;
    
    private readonly ObservableAsPropertyHelper<IEnumerable<SearchItem>> _searchItems;
    public IEnumerable<SearchItem> SearchItems => _searchItems.Value;

    public EditorViewModel()
    {
        _project = new();
        _searchItems = this.WhenValueChanged(x => x.SelectedSection)
            .Select(x => x == null ? [] : x.Items.Select(y => new SearchItem($"{y.Id}. {ObjectLocalizer.GetValue(y.Item)}", y)))
            .ToProperty(this, x => x.SearchItems);
        
        this.WhenActivated(disp =>
        {
            this.WhenValueChanged(x => x.SelectedSection).Subscribe(_ =>
                {
                    SelectedItem = null;
                    SelectedSearchItem = null;
                    SearchText = null;
                    JumpId = null;
                })
                .DisposeWith(disp);

            this.WhenValueChanged(x => x.SelectedSearchItem)
                .Throttle(DefaultThrottle)
                .WhereNotNull()
                .Subscribe(x => SelectedItem = x.TableItem)
                .DisposeWith(disp);
                
            _searchItems.DisposeWith(disp);

            _project.WhenAny(x => x.CurrentProject, x => x.Sender)
                .Subscribe(x =>
                {
                    Tables.Clear();
                    if (x.CurrentProject == null) return;
                    
                    var projDir = Path.GetDirectoryName(x.ProjectFile)!;
                    foreach (var tblFile in Directory.EnumerateFiles(projDir, "*.tbl", SearchOption.AllDirectories))
                    {
                        var tblName = Path.GetFileName(tblFile).ToUpperInvariant();
                        Table table;
                        switch (tblName)
                        {
                            case "ENCOUNT.TBL":
                                table = new(_project, new EncountTable(tblFile));
                                break;
                            case "SKILL.TBL":
                                table = new(_project, new SkillTable(_nameTableProxy, tblFile));
                                break;
                            case "PERSONA.TBL":
                                table = new(_project, new PersonaTable(_nameTableProxy, tblFile));
                                break;
                            case "PLAYER.TBL":
                                table = new(_project, new PlayerTable(tblFile));
                                break;
                            case "UNIT.TBL":
                                table = new(_project, new UnitTable(_nameTableProxy, tblFile));
                                break;
                            case "VISUAL.TBL":
                                table = new(_project, new VisualTable(_nameTableProxy, tblFile));
                                break;
                            case "ITEM.TBL":
                                table = new(_project, new ItemTable(_nameTableProxy, tblFile));
                                break;
                            case "NAME.TBL":
                                _nameTableProxy.NameTable = new(tblFile);
                                _nameTableFile = tblFile;
                                continue;
                            default: continue;
                        }

                        _tableFileMap[table] = tblFile;
                        Tables.Add(table);
                    }
                })
                .DisposeWith(disp);
        });
    }

    public Interaction<Unit, string?> SelectFolder { get; } = new();

    [ReactiveCommand]
    private async Task SelectProjectDir()
    {
        var selectedDir = await SelectFolder.Handle(new());
        if (string.IsNullOrEmpty(selectedDir)) return;
        
        _project.SetCurrentProject(selectedDir);
    }
    
    [ReactiveCommand]
    private async Task Save()
    {
        _project.Save();
        
        foreach (var table in _tables)
        {
            var tableFile = _tableFileMap[table];
            
            // Create backup.
            var tableBakFile = $"{tableFile}.bak";
            File.Copy(tableFile, tableBakFile, true);
            
            // Save file.
            await using var writer = new BigEndianBinaryWriter(File.Create(tableFile));
            table.Content.Write(writer);
        }

        if (_nameTableFile != null)
        {
            var nameBakFile = $"{_nameTableFile}.bak";
            File.Copy(_nameTableFile, nameBakFile, true);
            
            await using var fs = File.Create(_nameTableFile);
            _nameTableProxy.Write(fs);
        }
    }

    [ReactiveCommand]
    private void Jump()
    {
        if (JumpId == null || JumpId >= SelectedSection?.Items.Count)
        {
            return;
        }
        
        var item = SelectedSection?.Items[(int)JumpId];
        SelectedItem = item;
    }

    public record SearchItem(string Name, TableItem TableItem);   
}