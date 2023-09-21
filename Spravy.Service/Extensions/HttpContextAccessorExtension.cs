using Microsoft.AspNetCore.Http;
using Spravy.Domain.Extensions;

namespace Spravy.Service.Extensions;

public static class HttpContextAccessorExtension
{
    public static string GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext.ThrowIfNull();

        return httpContext.GetUserId();
    }
}