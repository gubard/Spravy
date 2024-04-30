using System.Runtime.CompilerServices;
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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    )
    {
        var userId = httpContextAccessor.HttpContext.ThrowIfNull().GetUserId();

        return new HttpHeaderItem(HttpNames.HeaderUserIdName, userId).ToReadOnlyMemory()
           .ToResult()
           .ToValueTaskResult()
           .ConfigureAwait(false);
    }
}