using Avalonia.Media;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class TextViewModel : ViewModelBase
{
    private string text = string.Empty;
    private bool acceptsReturn;
    private TextWrapping textWrapping;
    private string label;

    public string Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value);
    }

    public bool AcceptsReturn
    {
        get => acceptsReturn;
        set => this.RaiseAndSetIfChanged(ref acceptsReturn, value);
    }

    public TextWrapping TextWrapping
    {
        get => textWrapping;
        set => this.RaiseAndSetIfChanged(ref textWrapping, value);
    }

    public string Label
    {
        get => label;
        set => this.RaiseAndSetIfChanged(ref label, value);
    }
}