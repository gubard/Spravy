using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TimeZoneHttpHeaderFactory : IHttpHeaderFactory
{
    public Task<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        return HttpHeaderItem.TimeZoneOffset().ToReadOnlyMemory().ToResult().ToTaskResult();
    }
}