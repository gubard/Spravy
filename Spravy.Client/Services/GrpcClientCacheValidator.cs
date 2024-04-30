using Grpc.Core;
using Grpc.Net.Client;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Services;

public class GrpcClientCacheValidator<TGrpcClient> : ICacheValidator<Uri, TGrpcClient> where TGrpcClient : ClientBase
{
    private readonly ICache<Uri, GrpcChannel> grpcChannelCache;
    private readonly ICacheValidator<Uri, GrpcChannel> grpcChannelCacheValidator;

    public GrpcClientCacheValidator(
        ICacheValidator<Uri, GrpcChannel> grpcChannelCacheValidator,
        ICache<Uri, GrpcChannel> grpcChannelCache
    )
    {
        this.grpcChannelCacheValidator = grpcChannelCacheValidator;
        this.grpcChannelCache = grpcChannelCache;
    }

    public bool IsValid(Uri key, TGrpcClient value)
    {
        if (grpcChannelCache.TryGetCacheValue(key, out var channel))
        {
            return grpcChannelCacheValidator.IsValid(key, channel);
        }

        return true;
    }
}