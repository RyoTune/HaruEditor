namespace HaruEditor.Desktop.Project;

public interface ICommentService
{
    string GetComment(IComment commenter);
    void SetComment(IComment commenter, string? value);
}