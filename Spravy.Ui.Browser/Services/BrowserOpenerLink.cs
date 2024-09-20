namespace Spravy.Ui.Browser.Services;

public class BrowserOpenerLink : IOpenerLink
{
    public Cvtar OpenLinkAsync(Uri link, CancellationToken ct)
    {
        JsWindowInterop.WindowOpen(link.AbsoluteUri);

        return Result.AwaitableSuccess;
    }
}
