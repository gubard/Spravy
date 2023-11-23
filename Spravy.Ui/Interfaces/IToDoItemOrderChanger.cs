using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemOrderChanger : IRefresh
{
    IToDoService? ToDoService { get; }
}