namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemContentViewModel : NavigatableViewModelBase
{
    public ToDoItemContentViewModel()
        : base(true) { }

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
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
