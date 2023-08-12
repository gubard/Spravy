using Avalonia.Collections;
using ExtensionFramework.ReactiveUI.Models;
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