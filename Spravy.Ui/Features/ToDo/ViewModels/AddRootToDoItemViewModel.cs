namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddRootToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;

    public AddRootToDoItemViewModel(
        IObjectStorage objectStorage,
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent
    )
        : base(true)
    {
        this.objectStorage = objectStorage;
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
    }

    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }

    public override string ViewId
    {
        get => TypeCache<AddRootToDoItemViewModel>.Type.Name;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddRootToDoItemViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<AddRootToDoItemViewModelSetting>()
            .IfSuccessAsync(
                s =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        ToDoItemContent.Name = s.Name;
                        ToDoItemContent.Type = s.Type;
                        ToDoItemContent.Link = s.Link;
                        DescriptionContent.Description = s.Description;
                        DescriptionContent.Type = s.DescriptionType;

                        return Result.Success;
                    }),
                ct
            );
    }
}
