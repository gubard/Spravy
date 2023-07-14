using Grpc.Net.Client.Web;
using Spravy.Ui.Enums;

namespace Spravy.Ui.Models;

public class GrpcToDoServiceOptions
{
    public string Host { get; set; }
    public GrpcWebMode Mode { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}