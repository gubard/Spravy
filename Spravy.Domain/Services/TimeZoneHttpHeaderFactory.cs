using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class TimeZoneHttpHeaderFactory : IHttpHeaderFactory
{
    public Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync()
    {
        return HttpHeaderItem.TimeZoneOffset().ToEnumerable().ToTaskResult();
    }
}