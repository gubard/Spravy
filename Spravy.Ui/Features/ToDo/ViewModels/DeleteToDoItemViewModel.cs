namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class DeleteToDoItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    [ObservableProperty]
    private string childrenText = string.Empty;

    public DeleteToDoItemViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        DeleteItems = new();

        InitializedCommand = SpravyCommand.Create(
            ct =>
            {
                var statuses = Enum.GetValuesAsUnderlyingType<ToDoItemStatus>()
                    .OfType<ToDoItemStatus>()
                    .ToArray();

                return Result.AwaitableSuccess.IfSuccessAllAsync(
                    ct,
                    () =>
                    {
                        if (Item is null)
                        {
                            return Result.AwaitableSuccess;
                        }

                        return toDoService
                            .GetToDoItemAsync(Item.Id, ct)
                            .IfSuccessAsync(
                                i =>
                                    this.PostUiBackground(
                                        () => toDoCache.UpdateUi(i).ToResultOnly(),
                                        ct
                                    ),
                                ct
                            );
                    },
                    () =>
                    {
                        if (Item is null)
                        {
                            return Result.AwaitableSuccess;
                        }

                        return toDoService
                            .GetParentsAsync(Item.Id, ct)
                            .IfSuccessAsync(
                                parents =>
                                    this.PostUiBackground(
                                        () => toDoCache.UpdateParentsUi(Item.Id, parents),
                                        ct
                                    ),
                                ct
                            );
                    },
                    () =>
                    {
                        if (DeleteItems.IsEmpty())
                        {
                            return Item.IfNotNull(nameof(Item))
                                .IfSuccessAsync(
                                    item =>
                                        toDoService
                                            .ToDoItemToStringAsync(new(statuses, item.Id), ct)
                                            .IfSuccessAsync(
                                                text =>
                                                    this.PostUiBackground(
                                                        () =>
                                                        {
                                                            ChildrenText = text;

                                                            return Result.Success;
                                                        },
                                                        ct
                                                    ),
                                                ct
                                            ),
                                    ct
                                );
                        }

                        return DeleteItems
                            .ToReadOnlyMemory()
                            .ToResult()
                            .IfSuccessForEachAsync(
                                i =>
                                    toDoService
                                        .ToDoItemToStringAsync(new(statuses, i.Id), ct)
                                        .IfSuccessAsync(
                                            str =>
                                                $"{i.Name}{Environment.NewLine} {str.Split(Environment.NewLine).JoinString($"{Environment.NewLine} ")}".ToResult(),
                                            ct
                                        ),
                                ct
                            )
                            .IfSuccessAsync(
                                values =>
                                {
                                    var childrenText = string.Join(
                                        Environment.NewLine,
                                        values.ToArray()
                                    );

                                    return this.PostUiBackground(
                                        () =>
                                        {
                                            ChildrenText = childrenText;

                                            return Result.Success;
                                        },
                                        ct
                                    );
                                },
                                ct
                            );
                    }
                );
            },
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> DeleteItems { get; }
}
