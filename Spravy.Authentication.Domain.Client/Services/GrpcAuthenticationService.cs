using AutoMapper;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Domain.Client.Services;

public class GrpcAuthenticationService : GrpcServiceBase<AuthenticationServiceClient>,
    IAuthenticationService,
    IGrpcServiceCreator<GrpcAuthenticationService, AuthenticationServiceClient>
{
    private readonly IConverter converter;

    public GrpcAuthenticationService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        ISerializer serializer,
        IConverter converter
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
    }

    public Task<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                converter.Convert<UserGrpc>(user)
                    .IfSuccessAsync(
                        async userGrpc =>
                        {
                            var request = new LoginRequest
                            {
                                User = userGrpc,
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            var reply = await client.LoginAsync(request);

                            return converter.Convert<TokenResult>(reply);
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                converter.Convert<CreateUserRequest>(options)
                    .IfSuccessAsync(
                        async request =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await client.CreateUserAsync(request);

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<TokenResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                var reply = await client.RefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        RefreshToken = refreshToken,
                    }
                );

                return converter.Convert<TokenResult>(reply);
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateVerificationCodeByLoginAsync(
                    new UpdateVerificationCodeByLoginRequest
                    {
                        Login = login,
                    }
                );

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateVerificationCodeByEmailAsync(
                    new UpdateVerificationCodeByEmailRequest
                    {
                        Email = email
                    }
                );

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result<bool>> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                var reply = await client.IsVerifiedByLoginAsync(
                    new IsVerifiedByLoginRequest
                    {
                        Login = login
                    }
                );

                return reply.IsVerified.ToResult();
            },
            cancellationToken
        );
    }

    public Task<Result<bool>> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                var reply = await client.IsVerifiedByEmailAsync(
                    new IsVerifiedByEmailRequest
                    {
                        Email = email
                    }
                );

                return reply.IsVerified.ToResult();
            },
            cancellationToken
        );
    }

    public Task<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
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

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
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

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateEmailNotVerifiedUserByEmailAsync(
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

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateEmailNotVerifiedUserByLoginAsync(
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

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                var request = new DeleteUserByEmailRequest
                {
                    Email = email,
                    VerificationCode = verificationCode,
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.DeleteUserByEmailAsync(request, cancellationToken: cancellationToken);

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                var request = new DeleteUserByLoginRequest
                {
                    Login = login,
                    VerificationCode = verificationCode,
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.DeleteUserByLoginAsync(request, cancellationToken: cancellationToken);

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
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
                    }
                );

                return Result.Success;
            },
            cancellationToken
        );
    }

    public Task<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
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
                    }
                );

                return Result.Success;
            },
            cancellationToken
        );
    }

    public static GrpcAuthenticationService CreateGrpcService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        ISerializer serializer
    )
    {
        return new GrpcAuthenticationService(grpcClientFactory, host, serializer, converter);
    }
}