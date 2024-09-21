namespace Spravy.Ui.Services;

public class TopLevelClipboardService : IClipboardService
{
    private readonly Application app;

    public TopLevelClipboardService(Application app)
    {
        this.app = app;
    }

    public Cvtar SetTextAsync(string? text)
    {
        return SetTextCore(text).ConfigureAwait(false);
    }

    public async ValueTask<Result> SetTextCore(string? text)
    {
        await app.GetTopLevel().ThrowIfNull().Clipboard.ThrowIfNull().SetTextAsync(text);

        return Result.Success;
    }
}
