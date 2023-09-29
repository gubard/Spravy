using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Service.Model;

namespace Spravy.Service.Extensions;

public static class JwtBearerOptionsExtension
{
    public static JwtBearerOptions SetJwtOptions(this JwtBearerOptions options, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetConfigurationSection<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(jwtOptions.Key.ThrowIfNullOrWhiteSpace());
        var symmetricSecurityKey = new SymmetricSecurityKey(key);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer.ThrowIfNull(),
            ValidAudience = jwtOptions.Audience.ThrowIfNull(),
            IssuerSigningKey = symmetricSecurityKey,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
        };

        return options;
    }
}