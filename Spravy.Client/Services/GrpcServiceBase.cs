using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Spravy.Client.Enums;

namespace Spravy.Client.Services;

public class GrpcServiceBase : IAsyncDisposable, IDisposable
{
    protected readonly GrpcChannel GrpcChannel;

    protected GrpcServiceBase(Uri host, GrpcChannelType grpcChannelType, ChannelCredentials channelCredentials)
    {
        switch (grpcChannelType)
        {
            case GrpcChannelType.Default:
            {
                GrpcChannel = GrpcChannel.ForAddress(host);

                break;
            }
            case GrpcChannelType.GrpcWeb:
            {
                var httpClientHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler);

                var grpcChannelOptions = new GrpcChannelOptions()
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                GrpcChannel = GrpcChannel.ForAddress(host, grpcChannelOptions);

                break;
            }
            case GrpcChannelType.GrpcWebText:
            {
                var httpClientHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);

                var grpcChannelOptions = new GrpcChannelOptions()
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                GrpcChannel = GrpcChannel.ForAddress(host, grpcChannelOptions);

                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(grpcChannelType), grpcChannelType, null);
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        Dispose();
        await GrpcChannel.ShutdownAsync();
    }

    public virtual void Dispose()
    {
        GrpcChannel.Dispose();
    }
}