using Grpc.Core;

namespace Spravy.Core.Interfaces;

public interface IRpcExceptionHandler
{
    ConfiguredValueTaskAwaitable<Result> ToErrorAsync(RpcException exception, CancellationToken ct);
}
