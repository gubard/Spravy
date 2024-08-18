using System.Runtime.CompilerServices;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Mappers;
using Spravy.Authentication.Domain.Models;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Domain.Client.Services;

public class GrpcAuthenticationService
    : GrpcServiceBase<AuthenticationServiceClient>,
        IAuthenticationService,
        IGrpcServiceCreator<GrpcAuthenticationService, AuthenticationServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public GrpcAuthenticationService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
        : base(grpcClientFactory, host, handler, retryService) { }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(
        User user,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .LoginAsync(
                        new() { User = user.ToUserGrpc(), },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(reply => reply.ToTokenResult().ToResult(), ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .CreateUserAsync(
                        options.ToCreateUserRequest(),
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .RefreshTokenAsync(
                        new() { RefreshToken = refreshToken, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(reply => reply.ToTokenResult().ToResult(), ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByLoginAsync(
        string login,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateVerificationCodeByLoginAsync(
                        new() { Login = login, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByEmailAsync(
        string email,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateVerificationCodeByEmailAsync(
                        new() { Email = email, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(
        string login,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .IsVerifiedByLoginAsync(
                        new() { Login = login, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        reply =>
                            reply.IsVerified.ToResult().ToValueTaskResult().ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(
        string email,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .IsVerifiedByEmailAsync(
                        new() { Email = email, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        reply =>
                            reply.IsVerified.ToResult().ToValueTaskResult().ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .VerifiedEmailByLoginAsync(
                        new() { Login = login, VerificationCode = verificationCode, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .VerifiedEmailByEmailAsync(
                        new() { Email = email, VerificationCode = verificationCode, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateEmailNotVerifiedUserByEmailAsync(
                        new() { Email = email, NewEmail = newEmail, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateEmailNotVerifiedUserByLoginAsync(
                        new() { Login = login, NewEmail = newEmail, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .DeleteUserByEmailAsync(
                        new() { Email = email, VerificationCode = verificationCode, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .DeleteUserByLoginAsync(
                        new() { Login = login, VerificationCode = verificationCode, },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdatePasswordByEmailAsync(
                        new()
                        {
                            Email = email,
                            VerificationCode = verificationCode,
                            NewPassword = newPassword,
                        },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdatePasswordByLoginAsync(
                        new()
                        {
                            Login = login,
                            VerificationCode = verificationCode,
                            NewPassword = newPassword,
                        },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public static GrpcAuthenticationService CreateGrpcService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(grpcClientFactory, host, handler, retryService);
    }
}
