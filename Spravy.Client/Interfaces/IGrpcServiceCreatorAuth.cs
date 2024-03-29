using AutoMapper;
using Grpc.Core;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Interfaces;

public interface IGrpcServiceCreatorAuth<out TGrpcService, in TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    );
}

public interface IGrpcServiceCreator<out TGrpcService, in TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        ISerializer serializer
    );
}