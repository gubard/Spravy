using Spravy.Client.Services;
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
                        new() { User = user.ToUserGrpc() },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(reply => reply.ToTokenResult().ToResult(), ct),
            ct
        );
    }

    public Cvtar CreateUserAsync(CreateUserOptions options, CancellationToken ct)
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
                        new() { RefreshToken = refreshToken },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultValueOnly()
                    .ConfigureAwait(false)
                    .IfSuccessAsync(reply => reply.ToTokenResult().ToResult(), ct),
            ct
        );
    }

    public Cvtar UpdateVerificationCodeByLoginAsync(string login, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateVerificationCodeByLoginAsync(
                        new() { Login = login },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar UpdateVerificationCodeByEmailAsync(string email, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateVerificationCodeByEmailAsync(
                        new() { Email = email },
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
                        new() { Login = login },
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
                        new() { Email = email },
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

    public Cvtar VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .VerifiedEmailByLoginAsync(
                        new() { Login = login, VerificationCode = verificationCode },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .VerifiedEmailByEmailAsync(
                        new() { Email = email, VerificationCode = verificationCode },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateEmailNotVerifiedUserByEmailAsync(
                        new() { Email = email, NewEmail = newEmail },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                client
                    .UpdateEmailNotVerifiedUserByLoginAsync(
                        new() { Login = login, NewEmail = newEmail },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                client
                    .DeleteUserByEmailAsync(
                        new() { Email = email, VerificationCode = verificationCode },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                client
                    .DeleteUserByLoginAsync(
                        new() { Login = login, VerificationCode = verificationCode },
                        deadline: DateTime.UtcNow.Add(Timeout),
                        cancellationToken: ct
                    )
                    .ToValueTaskResultOnly()
                    .ConfigureAwait(false),
            ct
        );
    }

    public Cvtar UpdatePasswordByEmailAsync(
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

    public Cvtar UpdatePasswordByLoginAsync(
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
