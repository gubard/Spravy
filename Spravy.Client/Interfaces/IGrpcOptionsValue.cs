using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Interfaces;

public interface IGrpcOptionsValue : IOptionsValue
{
    public string? Host { get; }
    public GrpcChannelType ChannelType { get; }
    public ChannelCredentialType ChannelCredentialType { get; }
    public string? Token { get; }
}