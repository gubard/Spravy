namespace Spravy.Feedback.Domain.Models;

public readonly struct ErrorFeedback
{
    public ErrorFeedback(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
