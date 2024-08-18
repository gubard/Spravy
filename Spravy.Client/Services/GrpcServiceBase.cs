using Spravy.Client.Errors;
using Spravy.Core.Interfaces;

namespace Spravy.Client.Services;

public abstract class GrpcServiceBase<TGrpcClient>
    where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, TGrpcClient> grpcClientFactory;
    private readonly Uri host;
    private readonly IRpcExceptionHandler handler;
    private readonly IRetryService retryService;

    protected GrpcServiceBase(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        this.grpcClientFactory = grpcClientFactory;
        this.host = host;
        this.handler = handler;
        this.retryService = retryService;
    }

    protected ConfiguredValueTaskAwaitable<Result> CallClientAsync(
        Func<TGrpcClient, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        try
        {
            return retryService.TryAsync(
                () => grpcClientFactory.Create(host).IfSuccessAsync(func.Invoke, ct)
            );
        }
        catch (RpcException exception)
        {
            switch (exception.StatusCode)
            {
                case StatusCode.OK:
                    throw;
                case StatusCode.Cancelled:
                    return Result.AwaitableCanceledByUserError;
                case StatusCode.Unknown:
                    throw;
                case StatusCode.InvalidArgument:
                    return handler.ToErrorAsync(exception, ct);
                case StatusCode.DeadlineExceeded:
                    throw;
                case StatusCode.NotFound:
                    throw;
                case StatusCode.AlreadyExists:
                    throw;
                case StatusCode.PermissionDenied:
                    throw;
                case StatusCode.Unauthenticated:
                    throw;
                case StatusCode.ResourceExhausted:
                    throw;
                case StatusCode.FailedPrecondition:
                    throw;
                case StatusCode.Aborted:
                    throw;
                case StatusCode.OutOfRange:
                    throw;
                case StatusCode.Unimplemented:
                    throw;
                case StatusCode.Internal:
                    throw;
                case StatusCode.Unavailable:
                    return new Result(new ServiceUnavailableError(host.ToString()))
                        .ToValueTaskResult()
                        .ConfigureAwait(false);
                case StatusCode.DataLoss:
                    throw;
                default:
                    return new Result(new StatusCodeOutOfRangeError(exception.StatusCode))
                        .ToValueTaskResult()
                        .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
    }

    protected ConfiguredValueTaskAwaitable<Result<TValue>> CallClientAsync<TValue>(
        Func<TGrpcClient, ConfiguredValueTaskAwaitable<Result<TValue>>> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return CallClientCore(func, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result<TValue>> CallClientCore<TValue>(
        Func<TGrpcClient, ConfiguredValueTaskAwaitable<Result<TValue>>> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        try
        {
            return await retryService.TryAsync(
                () => grpcClientFactory.Create(host).IfSuccessAsync(func.Invoke, ct)
            );
        }
        catch (RpcException exception)
        {
            switch (exception.StatusCode)
            {
                case StatusCode.OK:
                    throw;
                case StatusCode.Cancelled:
                    return Result<TValue>.CanceledByUserError;
                case StatusCode.Unknown:
                    throw;
                case StatusCode.InvalidArgument:
                    var error = await handler.ToErrorAsync(exception, ct);

                    return error.Errors.ToResult<TValue>();
                case StatusCode.DeadlineExceeded:
                    throw;
                case StatusCode.NotFound:
                    throw;
                case StatusCode.AlreadyExists:
                    throw;
                case StatusCode.PermissionDenied:
                    throw;
                case StatusCode.Unauthenticated:
                    throw;
                case StatusCode.ResourceExhausted:
                    throw;
                case StatusCode.FailedPrecondition:
                    throw;
                case StatusCode.Aborted:
                    throw;
                case StatusCode.OutOfRange:
                    throw;
                case StatusCode.Unimplemented:
                    throw;
                case StatusCode.Internal:
                    throw;
                case StatusCode.Unavailable:
                    return new(new ServiceUnavailableError(host.ToString()));
                case StatusCode.DataLoss:
                    throw;
                default:
                    return new(new StatusCodeOutOfRangeError(exception.StatusCode));
            }
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
    }

    protected ConfiguredCancelableAsyncEnumerable<TResult> CallClientAsync<TResult>(
        Func<TGrpcClient, CancellationToken, ConfiguredCancelableAsyncEnumerable<TResult>> func,
        CancellationToken ct
    )
    {
        var client = grpcClientFactory.Create(host);

        return func.Invoke(client.ThrowIfError(), ct);
    }
}
