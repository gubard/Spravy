using Spravy.Core.Mappers;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;

    public AddToDoItemViewModel(
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IObjectStorage objectStorage,
        IToDoService toDoService
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
    }

    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }

    [Reactive]
    public object[] Path { get; set; } = [];

    [Reactive]
    public Guid ParentId { get; set; }

    public override string ViewId
    {
        get => $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{ParentId}";
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddToDoItemViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<AddToDoItemViewModelSetting>()
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

    public Result<AddToDoItemOptions> ConverterToAddToDoItemOptions()
    {
        return ConverterToAddToDoItemOptions(ParentId);
    }

    public Result<AddToDoItemOptions> ConverterToAddToDoItemOptions(Guid parentId)
    {
        if (ToDoItemContent.Link.IsNullOrWhiteSpace())
        {
            return new AddToDoItemOptions(
                parentId,
                ToDoItemContent.Name,
                ToDoItemContent.Type,
                DescriptionContent.Description,
                DescriptionContent.Type,
                new()
            ).ToResult();
        }

        return new AddToDoItemOptions(
            parentId,
            ToDoItemContent.Name,
            ToDoItemContent.Type,
            DescriptionContent.Description,
            DescriptionContent.Type,
            ToDoItemContent.Link.ToOptionUri()
        ).ToResult();
    }
}
