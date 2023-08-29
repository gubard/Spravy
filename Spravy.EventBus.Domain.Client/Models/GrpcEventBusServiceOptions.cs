using Spravy.Client.Enums;

namespace Spravy.EventBus.Domain.Client.Models;

public class GrpcEventBusServiceOptions
{
    public string Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}