namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : DialogableViewModelBase
{
    private readonly IObjectStorage objectStorage;

    public AddToDoItemViewModel(
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IObjectStorage objectStorage
    )
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
        Parent = null;
    }

    public AddToDoItemViewModel(
        ToDoItemEntityNotify parent,
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IObjectStorage objectStorage
    )
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
        Parent = parent;
    }

    public ToDoItemEntityNotify? Parent { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }

    public override string ViewId
    {
        get =>
            Parent is null
                ? $"{TypeCache<AddToDoItemViewModel>.Type.Name}"
                : $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{Parent.Id}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(ViewId, ct)
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
                            ToDoItemContent.Names.AddRange(setting.Names);

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddToDoItemViewModelSetting(this), ct);
    }

    public Result<AddToDoItemOptions> ConverterToAddToDoItemOptions()
    {
        if (Parent is null)
        {
            return new AddToDoItemOptions(
                new(),
                ToDoItemContent.Name,
                ToDoItemContent.Type,
                DescriptionContent.Description,
                DescriptionContent.DescriptionType,
                ToDoItemContent.Link.ToOptionUri()
            ).ToResult();
        }

        return ConverterToAddToDoItemOptions(Parent.Id);
    }

    public Result<AddToDoItemOptions> ConverterToAddToDoItemOptions(Guid parentId)
    {
        return new AddToDoItemOptions(
            parentId.ToOption(),
            ToDoItemContent.Name,
            ToDoItemContent.Type,
            DescriptionContent.Description,
            DescriptionContent.DescriptionType,
            ToDoItemContent.Link.ToOptionUri()
        ).ToResult();
    }
}
