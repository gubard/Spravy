using Spravy.Domain.Helpers;

namespace Spravy.Domain.Models;

public readonly struct HttpHeaderItem
{
    public HttpHeaderItem(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }

    public static HttpHeaderItem CreateBearerAuthorization(string token)
    {
        return new(HttpNames.HeaderAuthorizationName, $"{HttpNames.BearerAuthorizationName} {token}");
    }

    public static HttpHeaderItem CreateUserId(string userId)
    {
        return new(HttpNames.HeaderUserIdName, userId);
    }

    public static HttpHeaderItem TimeZoneOffset()
    {
        return new(HttpNames.HeaderTimeZoneOffsetName, DateTimeOffset.Now.Offset.ToString());
    }
}