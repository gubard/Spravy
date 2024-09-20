namespace Spravy.Domain.Interfaces;

public interface IEmailService
{
    Cvtar SendEmailAsync(string subject, string recipientEmail, string text, CancellationToken ct);
}
