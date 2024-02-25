using Avalonia.Collections;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ItemSelectorViewModel : ViewModelBase
{
    public AvaloniaList<object> Items { get; } = new();

    [Reactive]
    public object? SelectedItem { get; set; }
}