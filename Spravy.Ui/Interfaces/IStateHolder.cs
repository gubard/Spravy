namespace Spravy.Ui.Interfaces;

public interface IStateHolder
{
    Cvtar LoadStateAsync(CancellationToken ct);
    Cvtar SaveStateAsync(CancellationToken ct);
}
