using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
    Task LoginAsync(User user, CancellationToken cancellationToken);
    Task LoginAsync(string refreshToken, CancellationToken cancellationToken);
}