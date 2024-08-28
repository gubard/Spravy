namespace Spravy.Ui.Features.ToDo.ViewModels;

public class EditDescriptionViewModel : DialogableViewModelBase
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

    public override string ViewId
    {
        get => $"{TypeCache<EditDescriptionViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
