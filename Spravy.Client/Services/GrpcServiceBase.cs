using Grpc.Core;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
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

    protected async Task<Error> CallClientAsync(Func<TGrpcClient, Task> func, CancellationToken cancellationToken)
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();
            await func.Invoke(client);
        }
        catch (RpcException exception)
        {
            return await exception.ToErrorAsync(serializer);
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }

        return new Error();
    }

    protected async Task<TResult> CallClientAsync<TResult>(
        Func<TGrpcClient, Task<TResult>> func,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();
            var result = await func.Invoke(client);

            return result;
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