using System.Threading.Tasks;
using Avalonia.Collections;
using Spravy.Core.Interfaces;
using Spravy.Models;

namespace Spravy.Interfaces;

public interface IToDoItemOrderChanger
{
    IToDoService? ToDoService { get; }
    Task RefreshToDoItemAsync();
}