using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.Project;

public partial class ProjectConfig : ReactiveObject
{
    [Reactive] private string _id = Guid.NewGuid().ToString();
    
    public ProjectConfig() {}

    public Dictionary<string, string> Comments { get; init; } = [];

    public static ProjectConfig CreateOrLoadFromFile(string file)
    {
        if (File.Exists(file)) return FromFile(file);
        var newProj = new ProjectConfig();
        return newProj;
    }
    
    public static ProjectConfig FromFile(string file)
    {
        var projectConfig = JsonSerializer.Deserialize<ProjectConfig>(File.ReadAllBytes(file)) ?? throw new();
        return projectConfig;
    }
}