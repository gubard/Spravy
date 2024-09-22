namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public Cvtar SetTextAsync(string? text, CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
