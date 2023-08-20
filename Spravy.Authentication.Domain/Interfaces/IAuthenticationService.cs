using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<bool> IsValidAsync(User user);
    Task CreateUserAsync(CreateUserOptions options);
}