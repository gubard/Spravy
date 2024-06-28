namespace Spravy.Ui.Interfaces;

public interface IOpenerLink
{
    ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken ct);
}
