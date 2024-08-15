using Spravy.Core.Mappers;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;

    public AddToDoItemViewModel(
        ToDoItemEntityNotify parent,
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
        Parent = parent;

        Initialized = SpravyCommand.Create(
            ct =>
                objectStorage
                    .GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(ViewId, ct)
                    .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct),
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify Parent { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }
    public SpravyCommand Initialized { get; }

    public override string ViewId
    {
        get => $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{Parent.Id}";
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

    public Result<AddToDoItemOptions> ConverterToAddToDoItemOptions()
    {
        return ConverterToAddToDoItemOptions(Parent.Id);
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
                DescriptionContent.DescriptionType,
                new()
            ).ToResult();
        }

        return new AddToDoItemOptions(
            parentId,
            ToDoItemContent.Name,
            ToDoItemContent.Type,
            DescriptionContent.Description,
            DescriptionContent.DescriptionType,
            ToDoItemContent.Link.ToOptionUri()
        ).ToResult();
    }
}
