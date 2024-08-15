namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(CancellationToken ct);
    Cvtar NavigateToAsync(INavigatable parameter, CancellationToken ct);
}
