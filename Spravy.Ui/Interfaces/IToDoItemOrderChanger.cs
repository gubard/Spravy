using System.Threading.Tasks;

using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemOrderChanger
{
    IToDoService? ToDoService { get; }

    Task RefreshToDoItemAsync();
}