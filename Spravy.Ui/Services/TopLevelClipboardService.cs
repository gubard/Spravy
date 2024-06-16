namespace Spravy.Ui.Services;

public class TopLevelClipboardService : IClipboardService
{
    private readonly IClipboard clipboard;

    public TopLevelClipboardService(IClipboard clipboard)
    {
        this.clipboard = clipboard;
    }

    public ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text)
    {
        return SetTextCore(text).ConfigureAwait(false);
    }

    public async ValueTask<Result> SetTextCore(string? text)
    {
        await clipboard.SetTextAsync(text);

        return Result.Success;
    }
}