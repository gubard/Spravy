using System.Threading.Tasks;
using System.Windows.Input;

using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemOrderChanger
{
    IToDoService? ToDoService { get; }

    Task RefreshToDoItemAsync();
}