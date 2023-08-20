using Avalonia.Collections;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemHeaderViewModel : ViewModelBase
{
    private ToDoItemViewModel? item;

    public ToDoItemViewModel? Item
    {
        get => item;
        set => this.RaiseAndSetIfChanged(ref item, value);
    }

    public AvaloniaList<ToDoItemCommand> Commands { get; } = new();
}