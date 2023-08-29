using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Spravy.Client.Enums;

namespace Spravy.Client.Services;

public class GrpcServiceBase : IAsyncDisposable, IDisposable
{
    protected readonly GrpcChannel grpcChannel;
    private readonly GrpcChannelOptions grpcChannelOptions;
    private readonly GrpcWebHandler grpcWebHandler;
    private readonly HttpClientHandler httpClientHandler;

    protected GrpcServiceBase(Uri host, GrpcChannelType grpcChannelType, ChannelCredentials channelCredentials)
    {
        switch (grpcChannelType)
        {
            case GrpcChannelType.Default:
            {
                grpcChannel = GrpcChannel.ForAddress(host);
                break;
            }
            case GrpcChannelType.GrpcWeb:
            {
                httpClientHandler = new ();

                grpcWebHandler = new (GrpcWebMode.GrpcWeb, httpClientHandler);

                grpcChannelOptions = new ()
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                grpcChannel = GrpcChannel.ForAddress(host);
                break;
            }
            case GrpcChannelType.GrpcWebText:
            {
                httpClientHandler = new ();

                grpcWebHandler = new (GrpcWebMode.GrpcWebText, httpClientHandler);

                grpcChannelOptions = new ()
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                grpcChannel = GrpcChannel.ForAddress(host);
                
                break;
            }
            default: throw new ArgumentOutOfRangeException(nameof(grpcChannelType), grpcChannelType, null);
        }
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