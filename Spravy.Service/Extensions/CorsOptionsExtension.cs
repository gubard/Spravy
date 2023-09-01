using Microsoft.AspNetCore.Cors.Infrastructure;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class CorsOptionsExtension
{
    public static CorsOptions AddAllowAllPolicy(this CorsOptions options)
    {
        options.AddPolicy(
            PolicyNames.AllowAllName,
            policyBuilder => policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AddGrpcHeaders()
        );

        return options;
    }
}