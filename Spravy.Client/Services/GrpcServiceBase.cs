using Grpc.Core;
using Spravy.Client.Exceptions;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Services;

public abstract class GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, TGrpcClient> grpcClientFactory;
    private readonly Uri host;

    protected GrpcServiceBase(IFactory<Uri, TGrpcClient> grpcClientFactory, Uri host)
    {
        this.grpcClientFactory = grpcClientFactory;
        this.host = host;
    }

    protected async Task CallClientAsync(Func<TGrpcClient, Task> func, CancellationToken cancellationToken)
    {
        try
        {
            var client = grpcClientFactory.Create(host);
            cancellationToken.ThrowIfCancellationRequested();
            await func.Invoke(client);
        }
        catch (Exception e)
        {
            throw new GrpcException(host, e);
        }
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