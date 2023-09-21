using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;

namespace Spravy.Service.Extensions;

public static class HttpContextExtension
{
    public static Claim GetClaim(this HttpContext httpContext, string type)
    {
        return httpContext.User.Claims.GetClaim(type);
    }

    public static Claim GetNameIdentifierClaim(this HttpContext httpContext)
    {
        return httpContext.GetClaim(ClaimTypes.NameIdentifier);
    }

    public static Claim GetNameClaim(this HttpContext httpContext)
    {
        return httpContext.GetClaim(ClaimTypes.Name);
    }

    public static Claim GetRoleClaim(this HttpContext httpContext)
    {
        return httpContext.GetClaim(ClaimTypes.Role);
    }
    
    public static string GetUserId(this HttpContext httpContext)
    {
        var role = httpContext.GetRoleClaim().Value.ParseEnum<Role>();

        switch (role)
        {
            case Role.User:
            {
                var nameIdentifier = httpContext.GetNameIdentifierClaim();

                return nameIdentifier.Value;
            }
            case Role.Service:
            {
                var nameIdentifier = httpContext.Request.Headers[HttpNames.HeaderUserIdName].Single().ThrowIfNull();

                return nameIdentifier;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }
}