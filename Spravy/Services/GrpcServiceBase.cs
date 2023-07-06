using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace Spravy.Services;

public class GrpcServiceBase : IAsyncDisposable, IDisposable
{
    protected readonly GrpcChannel grpcChannel;

    protected GrpcServiceBase(Uri host)
    {
        grpcChannel = GrpcChannel.ForAddress(host);
    }

    public virtual async ValueTask DisposeAsync()
    {
        Dispose();
        await grpcChannel.ShutdownAsync();
    }

    public virtual void Dispose()
    {
        grpcChannel.Dispose();
    }
}