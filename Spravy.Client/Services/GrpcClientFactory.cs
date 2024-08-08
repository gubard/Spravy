namespace Spravy.Client.Services;

public class GrpcClientFactory<TGrpcClient> : IFactory<Uri, TGrpcClient>
    where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, GrpcChannel> grpcChannelFactory;
    private readonly IFactory<ChannelBase, TGrpcClient> grpcClientFactory;

    public GrpcClientFactory(
        IFactory<Uri, GrpcChannel> grpcChannelFactory,
        IFactory<ChannelBase, TGrpcClient> grpcClientFactory
    )
    {
        this.grpcChannelFactory = grpcChannelFactory;
        this.grpcClientFactory = grpcClientFactory;
    }

    public Result<TGrpcClient> Create(Uri key)
    {
        return grpcChannelFactory
            .Create(key)
            .IfSuccess(channel => grpcClientFactory.Create(channel));
    }
}
