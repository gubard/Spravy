using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Spravy.Authentication.Service.Services;

public class JwtTokenFactory : ITokenFactory
{
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;
    private readonly JwtTokenFactoryOptions options;

    public JwtTokenFactory(
        JwtTokenFactoryOptions options,
        JwtSecurityTokenHandler jwtSecurityTokenHandler
    )
    {
        this.options = options;
        this.jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    public Result<TokenResult> Create(UserTokenClaims user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Key.ThrowIfNullOrWhiteSpace())
        );
        var signingCredentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha512Signature
        );

        var jwt = CreateToken(
            signingCredentials,
            new()
            {
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.Email, user.Email),
            },
            DateTime.UtcNow.AddDays(options.ExpiresDays)
        );

        var refreshJwt = CreateToken(
            signingCredentials,
            new() { new(ClaimTypes.Name, user.Login), },
            DateTime.UtcNow.AddDays(options.RefreshExpiresDays)
        );

        return new TokenResult(jwt, refreshJwt).ToResult();
    }

    public Result<TokenResult> Create()
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Key.ThrowIfNullOrWhiteSpace())
        );
        var signingCredentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha512Signature
        );

        var jwt = CreateToken(
            signingCredentials,
            new() { new(ClaimTypes.Role, Role.Service.ToString()), },
            DateTime.UtcNow.AddDays(options.ExpiresDays)
        );

        var refreshJwt = CreateToken(
            signingCredentials,
            new() { new(ClaimTypes.Role, Role.Service.ToString()), },
            DateTime.UtcNow.AddDays(options.RefreshExpiresDays)
        );

        return new TokenResult(jwt, refreshJwt).ToResult();
    }

    private string CreateToken(
        SigningCredentials signingCredentials,
        List<Claim> claims,
        DateTime expires
    )
    {
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        var jwt = jwtSecurityTokenHandler.WriteToken(token);

        return jwt;
    }
}
