using Grpc.Core;
using Spravy.Domain.Models;

namespace Spravy.Client.Interfaces;

public interface IMetadataFactory
{
    Task<Result<Metadata>> CreateAsync(CancellationToken cancellationToken);
}