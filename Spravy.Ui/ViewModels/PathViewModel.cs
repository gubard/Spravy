using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PathViewModel : ViewModelBase
{
    public const string DefaultSeparator = ">";

    public AvaloniaList<object> Items { get; } = new();
}