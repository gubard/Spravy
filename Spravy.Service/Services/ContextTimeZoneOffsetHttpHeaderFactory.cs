using Microsoft.AspNetCore.Http;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;

namespace Spravy.Service.Services;

public class ContextTimeZoneOffsetHttpHeaderFactory : IHttpHeaderFactory
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ContextTimeZoneOffsetHttpHeaderFactory(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        var authorization = httpContextAccessor
            .HttpContext
            .ThrowIfNull()
            .GetTimeZoneOffsetHeader();

        return new HttpHeaderItem(HttpNames.HeaderTimeZoneOffsetName, authorization).ToReadOnlyMemory()
            .ToResult()
            .ToValueTaskResult();
    }
}