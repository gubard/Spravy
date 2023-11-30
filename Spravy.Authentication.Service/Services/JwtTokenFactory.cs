using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Models;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;

namespace Spravy.Authentication.Service.Services;

public class JwtTokenFactory : ITokenFactory
{
    private readonly JwtTokenFactoryOptions options;
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

    public JwtTokenFactory(JwtTokenFactoryOptions options, JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        this.options = options;
        this.jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    public TokenResult Create(UserTokenClaims user)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Key.ThrowIfNullOrWhiteSpace()));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = CreateToken(
            signingCredentials,
            new List<Claim>
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
            new List<Claim>
            {
                new(ClaimTypes.Name, user.Login),
            },
            DateTime.UtcNow.AddDays(options.RefreshExpiresDays)
        );

        return new TokenResult(jwt, refreshJwt);
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

    public TokenResult Create()
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Key.ThrowIfNullOrWhiteSpace()));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = CreateToken(
            signingCredentials,
            new List<Claim>
            {
                new(ClaimTypes.Role, Role.Service.ToString()),
            },
            DateTime.UtcNow.AddDays(options.ExpiresDays)
        );

        var refreshJwt = CreateToken(
            signingCredentials,
            new List<Claim>
            {
                new(ClaimTypes.Role, Role.Service.ToString()),
            },
            DateTime.UtcNow.AddDays(options.RefreshExpiresDays)
        );

        return new TokenResult(jwt, refreshJwt);
    }
}