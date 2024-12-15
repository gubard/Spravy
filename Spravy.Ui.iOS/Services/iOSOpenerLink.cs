using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.iOS.Services;

public class iOSOpenerLink : IOpenerLink
{
    public Cvtar OpenLinkAsync(Uri link, CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}