namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text)
    {
        return Result.AwaitableSuccess;
    }
}
