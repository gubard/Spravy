using Spravy.Ui.Enums;

namespace Spravy.Ui.Models;

public class GrpcAuthenticationServiceOptions
{
    public string Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}