namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public Cvtar SetTextAsync(string? text)
    {
        return Result.AwaitableSuccess;
    }
}
