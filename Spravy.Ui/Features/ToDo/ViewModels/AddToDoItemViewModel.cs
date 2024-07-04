using Spravy.Core.Mappers;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;

    public AddToDoItemViewModel(
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IObjectStorage objectStorage
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
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
        var setting = new AddToDoItemViewModelSetting(this);

        return objectStorage.SaveObjectAsync(ViewId, setting, ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<AddToDoItemViewModelSetting>()
            .IfSuccess(s =>
                this.PostUiBackground(() =>
                {
                    ToDoItemContent.Name = s.Name;
                    ToDoItemContent.Type = s.Type;
                    ToDoItemContent.Link = s.Link;
                    DescriptionContent.Description = s.Description;
                    DescriptionContent.Type = s.DescriptionType;

                    return Result.Success;
                })
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
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
