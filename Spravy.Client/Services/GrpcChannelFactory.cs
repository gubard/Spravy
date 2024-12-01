namespace Spravy.Client.Services;

public class GrpcChannelFactory : IFactory<Uri, GrpcChannel>
{
    private readonly ChannelCredentials channelCredentials;
    private readonly GrpcChannelType grpcChannelType;

    public GrpcChannelFactory(GrpcChannelType grpcChannelType, ChannelCredentials channelCredentials)
    {
        this.grpcChannelType = grpcChannelType;
        this.channelCredentials = channelCredentials;
    }

    public Result<GrpcChannel> Create(Uri key)
    {
        switch (grpcChannelType)
        {
            case GrpcChannelType.Default:
                return GrpcChannel.ForAddress(key).ToResult();
            case GrpcChannelType.GrpcWeb:
            {
                var httpClientHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler);

                var grpcChannelOptions = new GrpcChannelOptions
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                return GrpcChannel.ForAddress(key, grpcChannelOptions).ToResult();
            }
            case GrpcChannelType.GrpcWebText:
            {
                var httpClientHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);

                var grpcChannelOptions = new GrpcChannelOptions
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                return GrpcChannel.ForAddress(key, grpcChannelOptions).ToResult();
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(grpcChannelType), grpcChannelType, null);
        }
    }
}