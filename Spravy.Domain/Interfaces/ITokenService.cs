namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    ConfiguredValueTaskAwaitable<Result<string>> GetTokenAsync(CancellationToken ct);
    Cvtar LoginAsync(User user, CancellationToken ct);
    Cvtar LoginAsync(string refreshToken, CancellationToken ct);
}
