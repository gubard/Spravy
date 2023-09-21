using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Domain.Client.Models;

public class GrpcAuthenticationServiceOptions : IOptionsValue
{
    public static string Section => "GrpcAuthenticationService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}