using Grpc.Core;

namespace Spravy.PasswordGenerator.Domain.Client.Services;

public class PasswordServiceClientFactory : IFactory<ChannelBase, PasswordServiceClient>
{
    public Result<PasswordServiceClient> Create(ChannelBase key)
    {
        return new(new PasswordServiceClient(key));
    }
}