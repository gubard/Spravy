using AutoMapper;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Domain.Client.Services;

public class GrpcAuthenticationService : GrpcServiceBase, IAuthenticationService
{
    private readonly AuthenticationServiceClient client;
    private readonly IMapper mapper;

    public GrpcAuthenticationService(
        GrpcAuthenticationServiceOptions options,
        IMapper mapper
    )
        : base(
            options.Host.ThrowIfNullOrWhiteSpace().ToUri(),
            options.ChannelType,
            options.ChannelCredentialType.GetChannelCredentials()
        )
    {
        this.mapper = mapper;
        client = new AuthenticationServiceClient(GrpcChannel);
    }

    public async Task<TokenResult> LoginAsync(User user)
    {
        try
        {
            var userGrpc = mapper.Map<UserGrpc>(user);

            var request = new LoginRequest
            {
                User = userGrpc,
            };

            var reply = await client.LoginAsync(request);

            return mapper.Map<TokenResult>(reply);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task CreateUserAsync(CreateUserOptions options)
    {
        try
        {
            var request = mapper.Map<CreateUserRequest>(options);
            await client.CreateUserAsync(request);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<TokenResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var reply = await client.RefreshTokenAsync(
                new RefreshTokenRequest
                {
                    RefreshToken = refreshToken,
                }
            );

            return mapper.Map<TokenResult>(reply);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }
}