using Microsoft.AspNetCore.Http;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Service.Services;

public class ContextAccessorAuthorizationHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextAccessorAuthorizationHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        var authorization = httpContextAccessor
            .HttpContext
            .ThrowIfNull()
            .Request
            .Headers[HttpNames.HeaderAuthorizationName]
            .Single()
            .ThrowIfNull();

        return Task.FromResult(
            new HttpHeaderItem(HttpNames.HeaderAuthorizationName, authorization).ToEnumerable()
        );
    }
}