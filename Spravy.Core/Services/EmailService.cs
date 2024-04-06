using System.Runtime.CompilerServices;
using MailKit.Net.Smtp;
using MimeKit;
using Spravy.Core.Options;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Core.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions options;

    public EmailService(EmailOptions options)
    {
        this.options = options;
    }

    public ConfiguredValueTaskAwaitable<Result> SendEmailAsync(
        string subject,
        string recipientEmail,
        string text,
        CancellationToken cancellationToken
    )
    {
        return SendEmailCore(subject, recipientEmail, text, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> SendEmailCore(
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
            Text = text
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, 587, false, cancellationToken);
        await client.AuthenticateAsync(options.Login, options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        return Result.Success;
    }
}