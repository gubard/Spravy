namespace Spravy.Client.Interfaces;

public interface IGrpcServiceCreatorAuth<out TGrpcService, TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IConverter mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    );
}

public interface IGrpcServiceCreator<out TGrpcService, TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IConverter mapper,
        ISerializer serializer
    );
}