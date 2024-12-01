namespace Spravy.Core.Interfaces;

public interface IRpcExceptionHandler
{
    Cvtar ToErrorAsync(RpcException exception, CancellationToken ct);
}