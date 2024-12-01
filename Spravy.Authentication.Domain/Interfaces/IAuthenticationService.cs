namespace Spravy.Authentication.Domain.Interfaces;

public interface IAuthenticationService
{
    ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(User user, CancellationToken ct);
    Cvtar UpdateVerificationCodeByLoginAsync(string login, CancellationToken ct);
    Cvtar UpdateVerificationCodeByEmailAsync(string email, CancellationToken ct);
    Cvtar CreateUserAsync(CreateUserOptions options, CancellationToken ct);
    Cvtar VerifiedEmailByLoginAsync(string login, string verificationCode, CancellationToken ct);
    Cvtar VerifiedEmailByEmailAsync(string email, string verificationCode, CancellationToken ct);
    Cvtar DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken ct);
    Cvtar DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(string refreshToken, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(string login, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(string email, CancellationToken ct);

    Cvtar UpdateEmailNotVerifiedUserByEmailAsync(string email, string newEmail, CancellationToken ct);

    Cvtar UpdateEmailNotVerifiedUserByLoginAsync(string login, string newEmail, CancellationToken ct);

    Cvtar UpdatePasswordByEmailAsync(string email, string verificationCode, string newPassword, CancellationToken ct);

    Cvtar UpdatePasswordByLoginAsync(string login, string verificationCode, string newPassword, CancellationToken ct);
}