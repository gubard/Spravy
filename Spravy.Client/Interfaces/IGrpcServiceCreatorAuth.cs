using Spravy.Core.Interfaces;

namespace Spravy.Client.Interfaces;

public interface IGrpcServiceCreatorAuth<out TGrpcService, TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    );
}

public interface IGrpcServiceCreator<out TGrpcService, TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IRpcExceptionHandler handler
    );
}