namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemContentViewModel : NavigatableViewModelBase
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    public ToDoItemContentViewModel()
        : base(true) { }

    public override string ViewId
    {
        get => TypeCache<ToDoItemContentViewModel>.Type.Name;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
