namespace _build.Models;

public class ServiceOptions
{
    public ServiceOptions(ushort port, ushort browserPort, string serviceName, string browserServiceName)
    {
        Port = port;
        BrowserPort = browserPort;
        ServiceName = serviceName;
        BrowserServiceName = browserServiceName;
    }

    public ushort Port { get; }
    public ushort BrowserPort { get; }
    public string ServiceName { get; }
    public string BrowserServiceName { get; }
}