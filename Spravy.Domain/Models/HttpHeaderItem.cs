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
        return new HttpHeaderItem(HttpNames.HeaderAuthorizationName, $"{HttpNames.BearerAuthorizationName} {token}");
    }

    public static HttpHeaderItem CreateUserId(string userId)
    {
        return new HttpHeaderItem(HttpNames.HeaderUserIdName, userId);
    }

    public static HttpHeaderItem TimeZoneOffset()
    {
        return new HttpHeaderItem(HttpNames.HeaderTimeZoneOffsetName, DateTimeOffset.Now.Offset.ToString());
    }
}