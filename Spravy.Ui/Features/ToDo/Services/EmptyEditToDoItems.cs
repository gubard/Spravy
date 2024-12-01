namespace Spravy.Ui.Features.ToDo.Services;

public class EmptyEditToDoItems : IEditToDoItems
{
    public static EmptyEditToDoItems Default = new();

    public string ViewId => TypeCache<EmptyEditToDoItems>.Type.Name;

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