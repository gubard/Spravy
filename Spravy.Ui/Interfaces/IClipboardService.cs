namespace Spravy.Ui.Interfaces;

public interface IClipboardService
{
    ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text);
}