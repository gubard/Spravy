namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class DeleteToDoItemViewModel : ViewModelBase
{
    private readonly AvaloniaList<ToDoItemEntityNotify> items = new();

    [ObservableProperty]
    private string childrenText = string.Empty;

    public DeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService,
        IToDoUiService toDoUiService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Item = item;
        this.items.AddRange(items.ToArray());

        InitializedCommand = SpravyCommand.Create(
            ct =>
            {
                var statuses = UiHelper.ToDoItemStatuses;

                return Result.AwaitableSuccess.IfSuccessAllAsync(
                    ct,
                    () => toDoUiService.UpdateItemAsync(Item, ct),
                    () =>
                    {
                        if (Items.IsEmpty())
                        {
                            return Item.IfNotNull(nameof(Item))
                                .IfSuccessAsync(
                                    i =>
                                        toDoService
                                            .ToDoItemToStringAsync(new(statuses, i.Id), ct)
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

                        return Items
                            .ToArray()
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
                                    var text = string.Join(Environment.NewLine, values.ToArray());

                                    return this.PostUiBackground(
                                        () =>
                                        {
                                            ChildrenText = text;

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

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;
}
