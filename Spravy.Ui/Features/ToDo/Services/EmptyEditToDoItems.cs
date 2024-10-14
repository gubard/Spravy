namespace Spravy.Ui.Features.ToDo.Services;

public class EmptyEditToDoItems : IEditToDoItems
{
    public static EmptyEditToDoItems Default = new();

    public EditToDoItems GetEditToDoItems()
    {
        return new();
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
