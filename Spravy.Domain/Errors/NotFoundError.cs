namespace Spravy.Domain.Errors;

public class NotFoundError : Error
{
    public static readonly Guid MainId = new("288B59B8-C55E-49BA-8F1D-5CABFE78D206");

    protected NotFoundError() : base(MainId)
    {
        ErrorId = Guid.Empty;
    }

    public NotFoundError(Guid errorId) : base(MainId)
    {
        ErrorId = errorId;
    }

    public Guid ErrorId { get; protected set; }

    public override string Message => $"Not found error {ErrorId}";
}