using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken);
    Task CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken);
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}