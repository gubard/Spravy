using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IEmailService
{
    ConfiguredValueTaskAwaitable<Result> SendEmailAsync(
        string subject,
        string recipientEmail,
        string text,
        CancellationToken cancellationToken
    );
}