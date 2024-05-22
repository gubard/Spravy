namespace Spravy.Domain.Interfaces;

public interface IHttpHeaderFactory
{
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<HttpHeaderItem>>> CreateHeaderItemsAsync(
        CancellationToken cancellationToken
    );
}