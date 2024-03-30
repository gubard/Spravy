using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IHttpHeaderFactory
{
    Task<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken);
}