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
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService
                        .GetParentsAsync(ParentId, ct)
                        .IfSuccessAsync(
                            parents =>
                            {
                                var path = MaterialIconKind
                                    .Home.As<object>()
                                    .ToEnumerable()
                                    .Concat(parents.ToArray().Select(x => x.Name))
                                    .Select(x => x.ThrowIfNull())
                                    .ToArray();

                                return this.InvokeUiBackgroundAsync(() =>
                                {
                                    Path = path;

                                    return Result.Success;
                                });
                            },
                            ct
                        ),
                ct
            );
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

    [ProtoContract]
    private class AddToDoItemViewModelSetting : IViewModelSetting<AddToDoItemViewModelSetting>
    {
        static AddToDoItemViewModelSetting()
        {
            Default = new();
        }

        public AddToDoItemViewModelSetting() { }

        public AddToDoItemViewModelSetting(AddToDoItemViewModel viewModel)
        {
            Name = viewModel.ToDoItemContent.Name;
            Type = viewModel.ToDoItemContent.Type;
            Link = viewModel.ToDoItemContent.Link;
            Description = viewModel.DescriptionContent.Description;
            DescriptionType = viewModel.DescriptionContent.Type;
        }

        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(2)]
        public ToDoItemType Type { get; set; }

        [ProtoMember(3)]
        public string Link { get; set; } = string.Empty;

        [ProtoMember(4)]
        public string Description { get; set; } = string.Empty;

        [ProtoMember(5)]
        public DescriptionType DescriptionType { get; set; }

        public static AddToDoItemViewModelSetting Default { get; }
    }
}
