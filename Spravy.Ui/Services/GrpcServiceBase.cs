using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace Spravy.Ui.Services;

public class GrpcServiceBase : IAsyncDisposable, IDisposable
{
    protected readonly GrpcChannel grpcChannel;
    private readonly GrpcChannelOptions grpcChannelOptions;
    private readonly GrpcWebHandler grpcWebHandler;
    private readonly HttpClientHandler httpClientHandler;

    protected GrpcServiceBase(Uri host)
    {
        httpClientHandler = new();
        grpcWebHandler = new(GrpcWebMode.GrpcWeb, httpClientHandler);

        grpcChannelOptions = new()
        {
            HttpHandler = grpcWebHandler
        };

        grpcChannel = GrpcChannel.ForAddress(host, grpcChannelOptions);
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