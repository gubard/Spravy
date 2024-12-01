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

    public static string GetTimeZoneOffsetHeader(this HttpContext httpContext)
    {
        return httpContext.GetHeader(HttpNames.HeaderTimeZoneOffsetName);
    }

    public static string GetAuthorizationHeader(this HttpContext httpContext)
    {
        return httpContext.GetHeader(HttpNames.HeaderAuthorizationName);
    }

    public static TimeSpan GetTimeZoneOffset(this HttpContext httpContext)
    {
        return TimeSpan.Parse(httpContext.GetTimeZoneOffsetHeader());
    }

    public static string GetHeader(this HttpContext httpContext, string name)
    {
        return httpContext.Request.Headers[name].Single().ThrowIfNullOrWhiteSpace();
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}