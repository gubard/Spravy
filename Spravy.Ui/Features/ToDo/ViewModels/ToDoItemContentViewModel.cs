namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemContentViewModel : NavigatableViewModelBase
{
    public ToDoItemContentViewModel() : base(true)
    {
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; } = new(Enum.GetValues<ToDoItemType>());

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public ToDoItemType Type { get; set; }

    [Reactive]
    public string Link { get; set; } = string.Empty;

    public override string ViewId
    {
        get => TypeCache<ToDoItemContentViewModel>.Type.Name;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}