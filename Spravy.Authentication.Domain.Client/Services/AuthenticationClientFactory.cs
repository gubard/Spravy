using Grpc.Core;
using Spravy.Authentication.Protos;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Client.Services;

public class AuthenticationClientFactory
    : IFactory<ChannelBase, AuthenticationService.AuthenticationServiceClient>
{
    public Result<AuthenticationService.AuthenticationServiceClient> Create(ChannelBase key)
    {
        return new(new AuthenticationService.AuthenticationServiceClient(key));
    }
}
