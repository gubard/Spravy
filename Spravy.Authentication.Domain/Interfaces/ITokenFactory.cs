namespace Spravy.Authentication.Domain.Interfaces;

public interface ITokenFactory : IFactory<UserTokenClaims, TokenResult>, IFactory<TokenResult>
{
}