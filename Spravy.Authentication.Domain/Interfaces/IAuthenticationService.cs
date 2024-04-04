using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    ValueTask<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken);
    ValueTask<Result> CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken);
    ValueTask<Result<TokenResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    ValueTask<Result> UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken);
    ValueTask<Result> UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken);
    ValueTask<Result<bool>> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken);
    ValueTask<Result<bool>> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken);
    ValueTask<Result> VerifiedEmailByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);
    ValueTask<Result> VerifiedEmailByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    ValueTask<Result> UpdateEmailNotVerifiedUserByEmailAsync(string email, string newEmail, CancellationToken cancellationToken);
    ValueTask<Result> UpdateEmailNotVerifiedUserByLoginAsync(string login, string newEmail, CancellationToken cancellationToken);
    ValueTask<Result> DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    ValueTask<Result> DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);

    ValueTask<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );

    ValueTask<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );
}