using Grpc.Core;
using Grpc.Net.Client;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Services;

public class GrpcClientFactory<TGrpcClient> : IFactory<Uri, TGrpcClient> where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, GrpcChannel> grpcChannelFactory;

    public GrpcClientFactory(IFactory<Uri, GrpcChannel> grpcChannelFactory)
    {
        this.grpcChannelFactory = grpcChannelFactory;
    }

    public TGrpcClient Create(Uri key)
    {
        var channel = grpcChannelFactory.Create(key);
        var client = TypeCtorHelper<TGrpcClient, ChannelBase>.CtorFunc.Invoke(channel);

        return client;
    }
}