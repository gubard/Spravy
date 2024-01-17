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

    public Task UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateVerificationCodeByLoginAsync(
                    new UpdateVerificationCodeByLoginRequest
                    {
                        Login = login,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateVerificationCodeByEmailAsync(
                    new UpdateVerificationCodeByEmailRequest
                    {
                        Email = email
                    }
                );
            },
            cancellationToken
        );
    }

    public Task<bool> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.IsVerifiedByLoginAsync(
                    new IsVerifiedByLoginRequest
                    {
                        Login = login
                    }
                );

                return reply.IsVerified;
            },
            cancellationToken
        );
    }

    public Task<bool> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.IsVerifiedByEmailAsync(
                    new IsVerifiedByEmailRequest
                    {
                        Email = email
                    }
                );

                return reply.IsVerified;
            },
            cancellationToken
        );
    }

    public Task VerifiedEmailByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.VerifiedEmailByLoginAsync(
                    new VerifiedEmailByLoginRequest
                    {
                        Login = login,
                        VerificationCode = verificationCode,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task VerifiedEmailByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.VerifiedEmailByEmailAsync(
                    new VerifiedEmailByEmailRequest
                    {
                        Email = email,
                        VerificationCode = verificationCode,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateEmailNotVerifiedUserByEmailAsync(
                    new UpdateEmailNotVerifiedUserByEmailRequest
                    {
                        Email = email,
                        NewEmail = newEmail,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateEmailNotVerifiedUserByLoginAsync(
                    new UpdateEmailNotVerifiedUserByLoginRequest
                    {
                        Login = login,
                        NewEmail = newEmail,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string oldPassword,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordByEmailAsync(
                    new UpdatePasswordByEmailRequest
                    {
                        Email = email,
                        VerificationCode = verificationCode,
                        NewPassword = newPassword,
                        OldPassword = oldPassword,
                    }
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string oldPassword,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordByLoginAsync(
                    new UpdatePasswordByLoginRequest
                    {
                        Login = login,
                        VerificationCode = verificationCode,
                        NewPassword = newPassword,
                        OldPassword = oldPassword,
                    }
                );
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