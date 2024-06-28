using System.Runtime.CompilerServices;
using Spravy.Feedback.Domain.Models;

namespace Spravy.Feedback.Domain.Interfaces;

public interface IFeedback
{
    ConfiguredValueTaskAwaitable SendErrorAsync(ErrorFeedback feedback, CancellationToken ct);
}
