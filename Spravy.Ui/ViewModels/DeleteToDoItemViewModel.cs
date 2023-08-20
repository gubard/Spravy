using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    private ToDoSubItemNotify? item;

    public ToDoSubItemNotify? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }
}