using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface ITokenService
{
    Task<string> GetTokenAsync();
    Task LoginAsync(User user);
    Task LoginAsync(string refreshToken);
    void Login(TokenResult tokenResult);
}