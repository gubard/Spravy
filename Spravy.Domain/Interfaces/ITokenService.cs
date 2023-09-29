using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync();
    Task LoginAsync(User user);
    Task LoginAsync(string refreshToken);
}