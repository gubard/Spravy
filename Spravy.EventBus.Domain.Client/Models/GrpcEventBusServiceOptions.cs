using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.EventBus.Domain.Client.Models;

public class GrpcEventBusServiceOptions : IOptionsValue
{
    public static string Section => "GrpcEventBusService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}