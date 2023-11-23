using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design.Services;

public class OpenerLinkDesign : IOpenerLink
{
    public Task OpenLinkAsync(Uri link)
    {
        throw new NotImplementedException();
    }

    public Task OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}