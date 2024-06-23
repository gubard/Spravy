namespace Spravy.Ui.Interfaces;

public interface INavigator
{
    ConfiguredValueTaskAwaitable<Result<INavigatable>> NavigateBackAsync(CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        TViewModel parameter,
        CancellationToken cancellationToken
    ) where TViewModel : INavigatable;

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(
        Action<TViewModel> setup,
        CancellationToken cancellationToken
    ) where TViewModel : INavigatable;

    ConfiguredValueTaskAwaitable<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken)
        where TViewModel : INavigatable;
}