using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Client.Services;

public abstract class GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, TGrpcClient> grpcClientFactory;
    private readonly Uri host;
    private readonly ISerializer serializer;

    protected GrpcServiceBase(IFactory<Uri, TGrpcClient> grpcClientFactory, Uri host, ISerializer serializer)
    {
        this.grpcClientFactory = grpcClientFactory;
        this.host = host;
        this.serializer = serializer;
    }

    protected async ValueTask<Result> CallClientAsync(
        Func<TGrpcClient, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();

            return await func.Invoke(client);
        }
        catch (RpcException exception)
        {
            switch (exception.StatusCode)
            {
                case StatusCode.OK:
                    throw;
                case StatusCode.Cancelled:
                    throw;
                case StatusCode.Unknown:
                    throw;
                case StatusCode.InvalidArgument:
                    return await exception.ToErrorAsync(serializer);
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
                    return new Result(new ServiceUnavailableError(host.ToString()));
                case StatusCode.DataLoss:
                    throw;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
    }

    protected async ValueTask<Result<TValue>> CallClientAsync<TValue>(
        Func<TGrpcClient, ConfiguredValueTaskAwaitable<Result<TValue>>> func,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();

            return await func.Invoke(client);
        }
        catch (RpcException exception)
        {
            switch (exception.StatusCode)
            {
                case StatusCode.OK:
                    throw;
                case StatusCode.Cancelled:
                    throw;
                case StatusCode.Unknown:
                    throw;
                case StatusCode.InvalidArgument:
                    var error = await exception.ToErrorAsync(serializer);

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
                    return new Result<TValue>(new ServiceUnavailableError(host.ToString()));
                case StatusCode.DataLoss:
                    throw;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
    }

    protected IAsyncEnumerable<TResult> CallClientAsync<TResult>(
        Func<TGrpcClient, CancellationToken, IAsyncEnumerable<TResult>> func,
        CancellationToken cancellationToken
    )
    {
        var client = grpcClientFactory.Create(host);

        return func.Invoke(client, cancellationToken);
    }
}