namespace _build.Models;

public class ServiceOptions
{
    public ServiceOptions(ushort port, string serviceName)
    {
        Port = port;
        ServiceName = serviceName;
    }

    public ushort Port { get; }
    public string ServiceName { get; }
}