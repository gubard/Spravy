using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken);
    Task<Result> CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken);
    Task<Result<TokenResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Result> UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken);
    Task<Result> UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<bool>> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken);
    Task<Result<bool>> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result> VerifiedEmailByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);
    Task<Result> VerifiedEmailByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    Task<Result> UpdateEmailNotVerifiedUserByEmailAsync(string email, string newEmail, CancellationToken cancellationToken);
    Task<Result> UpdateEmailNotVerifiedUserByLoginAsync(string login, string newEmail, CancellationToken cancellationToken);
    Task<Result> DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    Task<Result> DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);

    Task<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );

    Task<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );
}