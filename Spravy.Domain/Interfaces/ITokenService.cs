using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    ValueTask<Result<string>> GetTokenAsync(CancellationToken cancellationToken);
    ValueTask<Result> LoginAsync(User user, CancellationToken cancellationToken);
    ValueTask<Result> LoginAsync(string refreshToken, CancellationToken cancellationToken);
}