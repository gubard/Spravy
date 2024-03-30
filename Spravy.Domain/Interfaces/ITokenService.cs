using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    Task<Result<string>> GetTokenAsync(CancellationToken cancellationToken);
    Task<Result> LoginAsync(User user, CancellationToken cancellationToken);
    Task<Result> LoginAsync(string refreshToken, CancellationToken cancellationToken);
}