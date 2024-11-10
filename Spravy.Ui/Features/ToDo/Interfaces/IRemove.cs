namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IRemove
{
    Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items);
}
