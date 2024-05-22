using System.Runtime.CompilerServices;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Client.Extensions;
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
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
    
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

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => converter.Convert<UserGrpc>(user)
           .IfSuccessAsync(userGrpc => client.LoginAsync(new()
                {
                    User = userGrpc,
                }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(reply => converter.Convert<TokenResult>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => converter.Convert<CreateUserRequest>(options)
               .IfSuccessAsync(request => client.CreateUserAsync(request, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken).ToValueTaskResultOnly().ConfigureAwait(false),
                    cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.RefreshTokenAsync(new()
            {
                RefreshToken = refreshToken,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultValueOnly()
           .ConfigureAwait(false)
           .IfSuccessAsync(reply => converter.Convert<TokenResult>(reply).ToValueTaskResult().ConfigureAwait(false),
                cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByLoginAsync(
        string login,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdateVerificationCodeByLoginAsync(new()
            {
                Login = login,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdateVerificationCodeByEmailAsync(new()
            {
                Email = email,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(
        string login,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.IsVerifiedByLoginAsync(new()
            {
                Login = login,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultValueOnly()
           .ConfigureAwait(false)
           .IfSuccessAsync(reply => reply.IsVerified.ToResult().ToValueTaskResult().ConfigureAwait(false),
                cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.IsVerifiedByEmailAsync(new()
            {
                Email = email,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultValueOnly()
           .ConfigureAwait(false)
           .IfSuccessAsync(reply => reply.IsVerified.ToResult().ToValueTaskResult().ConfigureAwait(false),
                cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.VerifiedEmailByLoginAsync(new()
            {
                Login = login,
                VerificationCode = verificationCode,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.VerifiedEmailByEmailAsync(new()
            {
                Email = email,
                VerificationCode = verificationCode,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdateEmailNotVerifiedUserByEmailAsync(new()
            {
                Email = email,
                NewEmail = newEmail,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdateEmailNotVerifiedUserByLoginAsync(new()
            {
                Login = login,
                NewEmail = newEmail,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.DeleteUserByEmailAsync(new()
            {
                Email = email,
                VerificationCode = verificationCode,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.DeleteUserByLoginAsync(new()
            {
                Login = login,
                VerificationCode = verificationCode,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdatePasswordByEmailAsync(new()
            {
                Email = email,
                VerificationCode = verificationCode,
                NewPassword = newPassword,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => client.UpdatePasswordByLoginAsync(new()
            {
                Login = login,
                VerificationCode = verificationCode,
                NewPassword = newPassword,
            }, deadline:DateTime.UtcNow.Add(Timeout),  cancellationToken:cancellationToken)
           .ToValueTaskResultOnly()
           .ConfigureAwait(false), cancellationToken);
    }

    public static GrpcAuthenticationService CreateGrpcService(
        IFactory<Uri, AuthenticationServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, serializer, converter);
    }
}