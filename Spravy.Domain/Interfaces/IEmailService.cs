namespace Spravy.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string subject, string recipientEmail, string text, CancellationToken cancellationToken);
}