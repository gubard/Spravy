namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemContentViewModel : NavigatableViewModelBase
{
    public ToDoItemContentViewModel()
        : base(true) { }

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private string link = string.Empty;

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
