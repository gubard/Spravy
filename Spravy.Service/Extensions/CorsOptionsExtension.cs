using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Spravy.Service.Extensions;

public static class CorsOptionsExtension
{
    public static CorsOptions AddAllowAllPolicy(this CorsOptions options)
    {
        options.AddDefaultPolicy(policyBuilder =>
            policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AddGrpcHeaders());

        return options;
    }
}