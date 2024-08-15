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

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddRootToDoItemViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            DescriptionContent.DescriptionType = setting.DescriptionType;
                            DescriptionContent.Description = setting.Description;
                            ToDoItemContent.Type = setting.Type;
                            ToDoItemContent.Link = setting.Link;
                            ToDoItemContent.Name = setting.Name;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddRootToDoItemViewModelSetting(this), ct);
    }
}
