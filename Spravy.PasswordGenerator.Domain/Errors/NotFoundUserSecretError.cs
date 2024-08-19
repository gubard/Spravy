namespace Spravy.PasswordGenerator.Domain.Errors;

public class NotFoundUserSecretError : Error
{
    public static readonly Guid MainId = new("DC84DA51-EBCC-4608-A1DF-4BFF0528CF90");

    protected NotFoundUserSecretError()
        : base(MainId) { }

    public NotFoundUserSecretError(Guid userId)
        : base(MainId)
    {
        UserId = userId;
    }

    public Guid UserId { get; protected set; }

    public override string Message
    {
        get => $"Not found secret for user \"{UserId}\".";
    }
}
