using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace Spravy.Ui.Services;

public class GrpcServiceBase : IAsyncDisposable, IDisposable
{
    protected readonly GrpcChannel grpcChannel;
    private readonly GrpcChannelOptions grpcChannelOptions;
    private readonly GrpcWebHandler grpcWebHandler;
    private readonly HttpClientHandler httpClientHandler;

    protected GrpcServiceBase(Uri host, GrpcWebMode mode, ChannelCredentials channelCredentials)
    {
        httpClientHandler = new();
        grpcWebHandler = new(mode, httpClientHandler);

        grpcChannelOptions = new()
        {
            HttpHandler = grpcWebHandler,
            Credentials = channelCredentials
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