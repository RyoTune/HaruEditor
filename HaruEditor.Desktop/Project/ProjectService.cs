using System.IO;
using System.Text.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.Project;

public partial class ProjectService : ReactiveObject, ICommentService
{
    private const string HaruFile = "project.haru";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    
    [Reactive(SetModifier = AccessModifier.Private)] private ProjectConfig? _currentProject;
    [Reactive(SetModifier = AccessModifier.Private)] private string? _projectFile;

    public void SetCurrentProject(string projectDir)
    {
        ProjectFile = Path.Join(projectDir, HaruFile);
        CurrentProject = ProjectConfig.CreateOrLoadFromFile(ProjectFile);
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(ProjectFile)) return;
        
        if (File.Exists(ProjectFile))
        {
            File.Copy(ProjectFile, $"{ProjectFile}.bak", true);
        }

        File.WriteAllText(ProjectFile, JsonSerializer.Serialize(_currentProject, JsonSerializerOptions));
    }

    public string GetComment(IComment commenter)
    {
        if (CurrentProject == null) return string.Empty;
        
        CurrentProject.Comments.TryGetValue(commenter.CommentId, out var comment);
        return comment ?? string.Empty;
    }

    public void SetComment(IComment commenter, string? value)
    {
        if (CurrentProject == null) return;
        CurrentProject.Comments[commenter.CommentId] = value ?? string.Empty;
    }
}