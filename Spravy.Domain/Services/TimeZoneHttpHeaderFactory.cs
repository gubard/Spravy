using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TimeZoneHttpHeaderFactory : IHttpHeaderFactory
{
    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        return HttpHeaderItem.TimeZoneOffset().ToEnumerable().ToTaskResult();
    }
}