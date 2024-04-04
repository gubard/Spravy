using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IHttpHeaderFactory
{
    ValueTask<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(CancellationToken cancellationToken);
}