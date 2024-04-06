using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Domain.Models;

namespace Spravy.Client.Interfaces;

public interface IMetadataFactory
{
    ConfiguredValueTaskAwaitable<Result<Metadata>> CreateAsync(CancellationToken cancellationToken);
}