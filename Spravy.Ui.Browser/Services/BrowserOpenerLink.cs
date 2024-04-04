using System;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Browser.Helpers;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Browser.Services;

public class BrowserOpenerLink : IOpenerLink
{
    public ValueTask<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        JSInterop.WindowOpen(link.AbsoluteUri);

        return Result.SuccessValueTask;
    }
}