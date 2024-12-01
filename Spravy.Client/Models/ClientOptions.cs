namespace Spravy.Client.Models;

public class ClientOptions
{
    public ClientOptions(bool useCache)
    {
        UseCache = useCache;
    }

    public bool UseCache { get; }
}