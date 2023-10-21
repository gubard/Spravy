using Microsoft.AspNetCore.Http;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;

namespace Spravy.Service.Services;

public class ContextAccessorUserIdHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextAccessorUserIdHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        var userId = httpContextAccessor.HttpContext.ThrowIfNull().GetUserId();

        return Task.FromResult(new HttpHeaderItem(HttpNames.HeaderUserIdName, userId).ToEnumerable());
    }
}