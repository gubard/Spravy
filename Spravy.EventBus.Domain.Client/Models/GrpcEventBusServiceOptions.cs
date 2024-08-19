namespace Spravy.EventBus.Domain.Client.Models;

public class GrpcEventBusServiceOptions : IGrpcOptionsValue
{
    public static string Section
    {
        get => "GrpcEventBusService";
    }

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}
