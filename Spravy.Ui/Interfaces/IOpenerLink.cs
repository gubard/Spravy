namespace Spravy.Ui.Interfaces;

public interface IOpenerLink
{
    Cvtar OpenLinkAsync(Uri link, CancellationToken ct);
}