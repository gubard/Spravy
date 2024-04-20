namespace Spravy.Domain.Errors;

public class UnknownError : Error
{
    public static readonly Guid MainId = new("D02032B7-8B2A-4164-A347-7C2441E6D96D");

    protected UnknownError() : base(MainId)
    {
    }

    public UnknownError(Guid errorId) : base(MainId)
    {
        ErrorId = errorId;
    }

    public Guid ErrorId { get; protected set; }

    public override string Message
    {
        get => $"Unknown error {ErrorId}";
    }
}