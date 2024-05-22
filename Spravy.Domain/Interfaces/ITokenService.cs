namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    ConfiguredValueTaskAwaitable<Result<string>> GetTokenAsync(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> LoginAsync(User user, CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> LoginAsync(string refreshToken, CancellationToken cancellationToken);
}