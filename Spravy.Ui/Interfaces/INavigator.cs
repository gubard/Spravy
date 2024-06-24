namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        TViewModel parameter,
        CancellationToken ct
    ) where TViewModel : INavigatable;

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        Action<TViewModel> setup,
        CancellationToken ct
    ) where TViewModel : INavigatable;

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(CancellationToken ct)
        where TViewModel : INavigatable;
}