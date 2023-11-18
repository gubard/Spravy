using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MainSplitViewModel : ViewModelBase
{
    private object? content;
    private object? pane;
    private bool isPaneOpen;

    public bool IsPaneOpen
    {
        get => isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref isPaneOpen, value);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public object? Pane
    {
        get => pane;
        set => this.RaiseAndSetIfChanged(ref pane, value);
    }
}