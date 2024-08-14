namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class EditDescriptionViewModel : ViewModelBase
{
    public EditDescriptionViewModel(
        ToDoItemEntityNotify item,
        EditDescriptionContentViewModel content
    )
    {
        Item = item;
        Content = content;
        Content.DescriptionType = item.DescriptionType;
        Content.Description = item.Description;
    }

    public ToDoItemEntityNotify Item { get; }
    public EditDescriptionContentViewModel Content { get; }
}
