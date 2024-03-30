using Grpc.Core;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
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

    protected async Task<Result> CallClientAsync(
        Func<TGrpcClient, Task<Result>> func,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();

            return await func.Invoke(client);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.InvalidArgument)
        {
            return await exception.ToErrorAsync(serializer);
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
    }

    protected async Task<Result<TValue>> CallClientAsync<TValue>(
        Func<TGrpcClient, Task<Result<TValue>>> func,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();

            return await func.Invoke(client);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.InvalidArgument)
        {
            var error = await exception.ToErrorAsync(serializer);

            return error.Errors.ToResult<TValue>();
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