using Grpc.Core;

namespace Spravy.Client.Interfaces;

public interface IMetadataFactory
{
    Task<Metadata> CreateAsync(CancellationToken cancellationToken);
}