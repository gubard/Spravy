using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Services;

public class GrpcChannelFactory : IFactory<Uri, GrpcChannel>
{
    private readonly GrpcChannelType grpcChannelType;
    private readonly ChannelCredentials channelCredentials;

    public GrpcChannelFactory(GrpcChannelType grpcChannelType, ChannelCredentials channelCredentials)
    {
        this.grpcChannelType = grpcChannelType;
        this.channelCredentials = channelCredentials;
    }

    public GrpcChannel Create(Uri key)
    {
        switch (grpcChannelType)
        {
            case GrpcChannelType.Default: return GrpcChannel.ForAddress(key);
            case GrpcChannelType.GrpcWeb:
            {
                var httpClientHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler);

                var grpcChannelOptions = new GrpcChannelOptions
                {
                    HttpHandler = grpcWebHandler,
                    Credentials = channelCredentials,
                };

                return GrpcChannel.ForAddress(key, grpcChannelOptions);
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

                return GrpcChannel.ForAddress(key, grpcChannelOptions);
            }
            default: throw new ArgumentOutOfRangeException(nameof(grpcChannelType), grpcChannelType, null);
        }
    }
}