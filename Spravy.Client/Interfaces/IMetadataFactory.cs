using Grpc.Core;
using Spravy.Domain.Models;

namespace Spravy.Client.Interfaces;

public interface IMetadataFactory
{
    ValueTask<Result<Metadata>> CreateAsync(CancellationToken cancellationToken);
}