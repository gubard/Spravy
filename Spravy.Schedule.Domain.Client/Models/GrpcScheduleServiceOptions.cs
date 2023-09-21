using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Schedule.Domain.Client.Models;

public class GrpcScheduleServiceOptions : IOptionsValue
{
    public static string Section => "GrpcScheduleService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}