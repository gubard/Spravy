using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainSplitViewModel : ViewModelBase, IContent
{
    [Reactive]
    public bool IsPaneOpen { get; set; }

    [Reactive]
    public object? Pane { get; set; }

    [Reactive]
    public object? Content { get; set; }
}