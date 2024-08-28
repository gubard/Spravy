namespace Spravy.Ui.Interfaces;

public interface IStateHolder
{
    string ViewId { get; }

    Cvtar LoadStateAsync(CancellationToken ct);
    Cvtar SaveStateAsync(CancellationToken ct);
}
