namespace Spravy.Ui.Interfaces;

public interface IRefresh
{
    Cvtar RefreshAsync(CancellationToken ct);
}