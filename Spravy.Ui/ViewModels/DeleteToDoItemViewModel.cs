using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    [Reactive]
    public string? ToDoItemName { get; set; }
}