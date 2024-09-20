using MailKit.Net.Smtp;
using MimeKit;

namespace Spravy.Authentication.Service.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions options;

    public EmailService(EmailOptions options)
    {
        this.options = options;
    }

    public Cvtar SendEmailAsync(
        string subject,
        string recipientEmail,
        string text,
        CancellationToken ct
    )
    {
        return SendEmailCore(subject, recipientEmail, text, ct).ConfigureAwait(false);
    }

    public async ValueTask<Result> SendEmailCore(
        string subject,
        string recipientEmail,
        string text,
        CancellationToken ct
    )
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Spravy", options.Login));
        message.To.Add(new MailboxAddress("User", recipientEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain") { Text = text };

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, 587, false, ct);
        await client.AuthenticateAsync(options.Login, options.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        return Result.Success;
    }
}
