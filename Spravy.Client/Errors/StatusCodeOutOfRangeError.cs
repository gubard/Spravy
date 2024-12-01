namespace Spravy.Client.Errors;

public class StatusCodeOutOfRangeError : ValueOutOfRangeError<StatusCode>
{
    public static readonly Guid MainId = new("64595D91-7D8C-4C7C-8141-5A9E22D0A82F");

    protected StatusCodeOutOfRangeError() : base(StatusCode.OK, MainId)
    {
    }

    public StatusCodeOutOfRangeError(StatusCode value) : base(value, MainId)
    {
    }
}