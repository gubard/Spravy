using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spravy.Domain.Extensions;
using Spravy.Service.Model;

namespace Spravy.Service.Extensions;

public static class JwtBearerOptionsExtension
{
    public static JwtBearerOptions SetJwtOptions(this JwtBearerOptions options, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.Section).Get<JwtOptions>().ThrowIfNull();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer.ThrowIfNull(),
            ValidAudience = jwtOptions.Audience.ThrowIfNull(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key.ThrowIfNull())),
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
        };

        return options;
    }
}