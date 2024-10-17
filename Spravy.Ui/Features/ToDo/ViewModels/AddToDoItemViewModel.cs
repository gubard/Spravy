namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;

    public AddToDoItemViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IObjectStorage objectStorage,
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        this.objectStorage = objectStorage;
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.toDoService = toDoService;
    }

    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }

    public override string ViewId
    {
        get =>
            EditItem.TryGetValue(out var editItem)
                ? $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{editItem.Id}"
                : $"{TypeCache<AddToDoItemViewModel>.Type.Name}";
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

    private AddToDoItemOptions ConverterToAddToDoItemOptions()
    {
        return new(
            new(),
            ToDoItemContent.Name,
            ToDoItemContent.Type,
            DescriptionContent.Description,
            DescriptionContent.DescriptionType,
            ToDoItemContent.Link.ToOptionUri(),
            new(),
            ToDoItemContent.Icon,
            ToDoItemContent.Color.ToString()
        );
    }

    private AddToDoItemOptions ConverterToAddToDoItemOptions(Guid parentId)
    {
        return new(
            parentId.ToOption(),
            ToDoItemContent.Name,
            ToDoItemContent.Type,
            DescriptionContent.Description,
            DescriptionContent.DescriptionType,
            ToDoItemContent.Link.ToOptionUri(),
            new(),
            ToDoItemContent.Icon,
            ToDoItemContent.Color.ToString()
        );
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return Result
            .AwaitableSuccess.IfSuccessAsync(
                () =>
                {
                    if (ResultIds.IsEmpty)
                    {
                        return toDoService.AddToDoItemAsync(
                            new[] { ConverterToAddToDoItemOptions() },
                            ct
                        );
                    }

                    return ResultIds
                        .ToResult()
                        .IfSuccessForEach(x => ConverterToAddToDoItemOptions(x).ToResult())
                        .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct);
                },
                ct
            )
            .ToResultOnlyAsync();
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
