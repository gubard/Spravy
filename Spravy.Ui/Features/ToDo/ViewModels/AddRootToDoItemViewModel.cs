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
            .IfSuccess(x =>
                this.PostUiBackground(
                    () =>
                    {
                        DescriptionContent.DescriptionType = x.DescriptionType;
                        DescriptionContent.Description = x.Description;
                        ToDoItemContent.Link = x.Link;
                        ToDoItemContent.Name = x.Name;
                        ToDoItemContent.Type = x.Type;

                        return Result.Success;
                    },
                    ct
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }
}
