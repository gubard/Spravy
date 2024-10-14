namespace Spravy.Ui.Features.ToDo.Services;

public class EmptyToDoItemEditId : IToDoItemEditId
{
    public static EmptyToDoItemEditId Default = new();

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        return new ToDoItemEditId().ToResult();
    }
}
