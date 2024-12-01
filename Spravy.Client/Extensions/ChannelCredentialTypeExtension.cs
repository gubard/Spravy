namespace Spravy.Client.Extensions;

public static class ChannelCredentialTypeExtension
{
    public static ChannelCredentials GetChannelCredentials(this ChannelCredentialType type)
    {
        return type switch
        {
            ChannelCredentialType.SecureSsl => ChannelCredentials.SecureSsl,
            ChannelCredentialType.Insecure => ChannelCredentials.Insecure,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}