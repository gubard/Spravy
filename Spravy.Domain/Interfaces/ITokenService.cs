namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    ConfiguredValueTaskAwaitable<Result<string>> GetTokenAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> LoginAsync(User user, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result> LoginAsync(string refreshToken, CancellationToken ct);
}