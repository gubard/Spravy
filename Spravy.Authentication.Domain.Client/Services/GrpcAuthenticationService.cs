using AutoMapper;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Domain.Client.Services;

public class GrpcAuthenticationService : GrpcServiceBase<AuthenticationServiceClient>,
    IAuthenticationService,
    IGrpcServiceCreator2<GrpcAuthenticationService, AuthenticationServiceClient>
{
    private readonly IMapper mapper;

    public GrpcAuthenticationService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper
    ) : base(grpcClientFactory, host)
    {
        this.mapper = mapper;
    }

    public Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                var userGrpc = mapper.Map<UserGrpc>(user);

                var request = new LoginRequest
                {
                    User = userGrpc,
                };

                cancellationToken.ThrowIfCancellationRequested();
                var reply = await client.LoginAsync(request);

                return mapper.Map<TokenResult>(reply);
            },
            cancellationToken
        );
    }

    public Task CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                var request = mapper.Map<CreateUserRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();
                await client.CreateUserAsync(request);
            },
            cancellationToken
        );
    }

    public Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.RefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        RefreshToken = refreshToken,
                    }
                );

                return mapper.Map<TokenResult>(reply);
            },
            cancellationToken
        );
    }

    public static GrpcAuthenticationService CreateGrpcService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper
    )
    {
        return new GrpcAuthenticationService(grpcClientFactory, host, mapper);
    }
}