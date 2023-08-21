using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Models;

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
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(options.ExpiresDays),
            signingCredentials: signingCredentials
        );

        var jwt = jwtSecurityTokenHandler.WriteToken(token);

        return new TokenResult(jwt);
    }
}