namespace Spravy.Domain.Errors;

public class ServiceUnavailableError : Error
{
    public static readonly Guid MainId = new("3808254B-2088-42D3-8A24-9125E286B6D4");

    protected ServiceUnavailableError() : base(MainId)
    {
        ServerHost = string.Empty;
    }

    public ServiceUnavailableError(string serverHost) : base(MainId)
    {
        ServerHost = serverHost;
    }

    public string ServerHost { get; protected set; }

    public override string Message => $"Service {ServerHost} unavailable";
}