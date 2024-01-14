using MailKit.Net.Smtp;
using MimeKit;
using Spravy.Core.Options;
using Spravy.Domain.Interfaces;

namespace Spravy.Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions options;

    public EmailService(EmailOptions options)
    {
        this.options = options;
    }

    public async Task SendEmailAsync(
        string subject,
        string recipientEmail,
        string text,
        CancellationToken cancellationToken
    )
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Spravy", options.Login));
        message.To.Add(new MailboxAddress("User", recipientEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = subject
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, 465, true, cancellationToken);
        await client.AuthenticateAsync(options.Login, options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}