namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
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
                var statuses = Enum.GetValues<ToDoItemStatus>();

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
                                i => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(i)),
                                ct
                            )
                            .ToResultOnlyAsync();
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
                                    this.InvokeUiBackgroundAsync(
                                        () => toDoCache.UpdateParentsUi(Item.Id, parents)
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
                                                    this.InvokeUiBackgroundAsync(() =>
                                                    {
                                                        ChildrenText = text;

                                                        return Result.Success;
                                                    }),
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

                                    return this.InvokeUiBackgroundAsync(() =>
                                    {
                                        ChildrenText = childrenText;

                                        return Result.Success;
                                    });
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

    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }

    [Reactive]
    public string ChildrenText { get; set; } = string.Empty;
}
