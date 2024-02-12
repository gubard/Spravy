using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    private string? toDoItemName;

    public string? ToDoItemName
    {
        get => toDoItemName;
        set => this.RaiseAndSetIfChanged(ref toDoItemName, value);
    }
}