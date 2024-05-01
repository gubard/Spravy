namespace Spravy.Ui.Services;

public class TopLevelClipboardService : IClipboardService
{
    private readonly IClipboard clipboard = Application.Current
       .ThrowIfNull("Application")
       .GetTopLevel()
       .ThrowIfNull("TopLevel")
       .Clipboard
       .ThrowIfNull();

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