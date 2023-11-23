using System;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Ui.Browser.Helpers;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Browser.Services;

public class BrowserOpenerLink : IOpenerLink
{
    public Task OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        JSInterop.WindowOpen(link.AbsoluteUri);

        return Task.CompletedTask;
    }
}