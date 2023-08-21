using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResult> LoginAsync(User user);
    Task CreateUserAsync(CreateUserOptions options);
}