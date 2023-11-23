using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IHttpHeaderFactory
{
    Task<IEnumerable<HttpHeaderItem>> CreateHeaderItemsAsync(CancellationToken cancellationToken);
}