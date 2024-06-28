namespace Spravy.Ui.ViewModels;

public class TextViewModel : ViewModelBase
{
    [Reactive]
    public bool IsReadOnly { get; set; }

    [Reactive]
    public string Text { get; set; } = string.Empty;

    [Reactive]
    public bool AcceptsReturn { get; set; }

    [Reactive]
    public TextWrapping TextWrapping { get; set; }

    [Reactive]
    public string Label { get; set; } = string.Empty;
}
