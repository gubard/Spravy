using AutoMapper;
using Grpc.Core;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Interfaces;

public interface IGrpcServiceCreator<out TGrpcService, in TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    );
}

public interface IGrpcServiceCreator2<out TGrpcService, in TGrpcClient>
    where TGrpcService : GrpcServiceBase<TGrpcClient> where TGrpcClient : ClientBase
{
    static abstract TGrpcService CreateGrpcService(
        IFactory<Uri, TGrpcClient> grpcClientFactory,
        Uri host,
        IMapper mapper
    );
}