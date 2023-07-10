using System.Threading.Tasks;
using Spravy.Core.Interfaces;

namespace Spravy.Interfaces;

public interface IToDoItemOrderChanger
{
    IToDoService? ToDoService { get; }
    Task RefreshToDoItemAsync();
}