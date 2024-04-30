using System.Runtime.CompilerServices;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserOptions options,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByLoginAsync(
        string login,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByEmailAsync(
        string email,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(
        string login,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(
        string email,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );
}