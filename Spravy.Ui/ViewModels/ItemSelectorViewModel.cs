using Avalonia.Collections;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ItemSelectorViewModel : ViewModelBase
{
    private object? selectedItem;
    
    public AvaloniaList<object> Items { get; } = new();

    public object? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }
}