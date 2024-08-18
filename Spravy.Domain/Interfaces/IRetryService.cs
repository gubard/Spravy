namespace Spravy.Domain.Interfaces;

public interface IRetryService
{
    ConfiguredValueTaskAwaitable<Result<TReturn>> TryAsync<TReturn>(
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
        where TReturn : notnull;

    ConfiguredValueTaskAwaitable<Result> TryAsync(Func<ConfiguredValueTaskAwaitable<Result>> func);
}
