namespace Spravy.Ui.Interfaces;

public interface IClipboardService
{
    Cvtar SetTextAsync(string? text, CancellationToken ct);
}