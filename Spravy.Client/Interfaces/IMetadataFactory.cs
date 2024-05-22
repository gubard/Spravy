namespace Spravy.Client.Interfaces;

public interface IMetadataFactory
{
    ConfiguredValueTaskAwaitable<Result<Metadata>> CreateAsync(CancellationToken cancellationToken);
}