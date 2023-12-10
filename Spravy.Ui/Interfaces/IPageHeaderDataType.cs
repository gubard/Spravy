using System.Windows.Input;
using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.Interfaces;

public interface IPageHeaderDataType
{
    ToDoItemCommand? LeftCommand { get; }
    ToDoItemCommand? RightCommand { get; }
    ICommand SwitchPaneCommand { get; }
    object? Header { get; }
    AvaloniaList<ToDoItemCommand> Commands { get; }
}