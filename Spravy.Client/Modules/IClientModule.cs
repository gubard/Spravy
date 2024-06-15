using Jab;

namespace Spravy.Client.Modules;

[ServiceProviderModule]
[Transient(typeof(ICacheValidator<Uri, GrpcChannel>), typeof(GrpcChannelCacheValidator))]
public interface IClientModule
{
    
}