namespace Spravy.Client.Extensions;

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