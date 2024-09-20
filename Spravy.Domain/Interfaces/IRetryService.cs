namespace Spravy.Domain.Interfaces;

public interface IRetryService
{
    ConfiguredValueTaskAwaitable<Result<TReturn>> TryAsync<TReturn>(
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
        where TReturn : notnull;

    Cvtar TryAsync(Func<Cvtar> func);
}
