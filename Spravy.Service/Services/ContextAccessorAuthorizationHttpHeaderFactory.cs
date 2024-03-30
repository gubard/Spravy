using Microsoft.AspNetCore.Http;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;

namespace Spravy.Service.Services;

public class ContextAccessorAuthorizationHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextAccessorAuthorizationHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        var authorization = httpContextAccessor
            .HttpContext
            .ThrowIfNull()
            .GetAuthorizationHeader();

        return new HttpHeaderItem(HttpNames.HeaderAuthorizationName, authorization).ToReadOnlyMemory()
            .ToResult()
            .ToTaskResult();
    }
}