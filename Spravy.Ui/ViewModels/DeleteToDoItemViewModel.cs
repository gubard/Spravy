using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    private ToDoItemNotify? item;

    public ToDoItemNotify? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }
}