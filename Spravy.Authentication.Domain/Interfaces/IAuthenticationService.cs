using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken);
    Task<Result> CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken);
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken);
    Task UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken);
    Task<bool> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken);
    Task VerifiedEmailByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);
    Task VerifiedEmailByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    Task UpdateEmailNotVerifiedUserByEmailAsync(string email, string newEmail, CancellationToken cancellationToken);
    Task UpdateEmailNotVerifiedUserByLoginAsync(string login, string newEmail, CancellationToken cancellationToken);
    Task DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken cancellationToken);
    Task DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken cancellationToken);

    Task UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );

    Task UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    );
}