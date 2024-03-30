using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class EmptyHttpHeaderFactory : IHttpHeaderFactory
{
    public Task<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken)
    {
        return ReadOnlyMemory<HttpHeaderItem>.Empty.ToResult().ToTaskResult();
    }
}