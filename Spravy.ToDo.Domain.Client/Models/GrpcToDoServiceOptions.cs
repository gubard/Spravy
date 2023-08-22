using Spravy.ToDo.Domain.Client.Enums;

namespace Spravy.ToDo.Domain.Client.Models;

public class GrpcToDoServiceOptions
{
    public string Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
}