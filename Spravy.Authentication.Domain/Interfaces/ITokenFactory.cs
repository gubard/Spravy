using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Domain.Interfaces;

public interface ITokenFactory : IFactory<UserTokenClaims, TokenResult>, IFactory<TokenResult> { }
