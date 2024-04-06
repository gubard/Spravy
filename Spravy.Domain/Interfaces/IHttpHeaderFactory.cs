using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IHttpHeaderFactory
{
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    );
}