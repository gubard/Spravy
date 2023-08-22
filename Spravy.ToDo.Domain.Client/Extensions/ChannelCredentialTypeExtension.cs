using Grpc.Core;
using Spravy.ToDo.Domain.Client.Enums;

namespace Spravy.ToDo.Domain.Client.Extensions;

public static class ChannelCredentialTypeExtension
{
    public static ChannelCredentials GetChannelCredentials(this ChannelCredentialType type)
    {
        switch (type)
        {
            case ChannelCredentialType.SecureSsl:
                return ChannelCredentials.SecureSsl;
            case ChannelCredentialType.Insecure:
                return ChannelCredentials.Insecure;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}