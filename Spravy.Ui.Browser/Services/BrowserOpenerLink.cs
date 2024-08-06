using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Ui.Browser.Helpers;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Browser.Services;

public class BrowserOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken ct)
    {
        JsWindowInterop.WindowOpen(link.AbsoluteUri);

        return Result.AwaitableSuccess;
    }
}
