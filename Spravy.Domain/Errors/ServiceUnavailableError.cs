namespace Spravy.Domain.Errors;

public class ServiceUnavailableError : Error
{
    public static readonly Guid MainId = new("3808254B-2088-42D3-8A24-9125E286B6D4");
    
    public ServiceUnavailableError(string serverPath) : base(MainId, $"Service {serverPath} unavailable.")
    {
        ServerPath = serverPath;
    }

    public string ServerPath { get; protected set; }
}