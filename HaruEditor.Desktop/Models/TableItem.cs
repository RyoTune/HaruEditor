using HaruEditor.Desktop.Project;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace HaruEditor.Desktop.Models;

public partial class TableItem(ICommentService comments, int id, object item) : ReactiveObject, IComment
{
    [Reactive] private ObjectWithId _item = new(id, item);
    [Reactive] private int _id = id;
    [Reactive] private string _tags = string.Empty;

    public string CommentId { get; } = $"{item.GetType().FullName}.{id}";
    
    public string Comment
    {
        get => comments.GetComment(this);
        set
        {
            comments.SetComment(this, value);
            this.RaisePropertyChanged();
        }
    }
}