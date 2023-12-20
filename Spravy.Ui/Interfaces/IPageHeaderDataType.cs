using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.Interfaces;

public interface IPageHeaderDataType
{
    CommandItem? LeftCommand { get; }
    CommandItem? RightCommand { get; }
    object? Header { get; }
    AvaloniaList<CommandItem> Commands { get; }
}