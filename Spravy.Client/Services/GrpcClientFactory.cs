namespace Spravy.Client.Services;

public class GrpcClientFactory<TGrpcClient> : IFactory<Uri, TGrpcClient> where TGrpcClient : ClientBase
{
    private readonly IFactory<Uri, GrpcChannel> grpcChannelFactory;

    public GrpcClientFactory(IFactory<Uri, GrpcChannel> grpcChannelFactory)
    {
        this.grpcChannelFactory = grpcChannelFactory;
    }

    public Result<TGrpcClient> Create(Uri key)
    {
        return grpcChannelFactory.Create(key)
           .IfSuccess(channel => TypeCtorHelper<TGrpcClient, ChannelBase>.CtorFunc.Invoke(channel).ToResult());
    }
}