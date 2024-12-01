namespace Spravy.Ui.Features.ToDo.Services;

public class EmptyToDoItemSettings : IToDoItemSettings
{
    public static readonly EmptyToDoItemSettings Default = new();

    public string ViewId => TypeCache<EmptyToDoItemSettings>.Type.Name;

    public EditToDoItems GetEditToDoItems()
    {
        return new();
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}