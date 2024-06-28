namespace Spravy.Integration.Tests.Extensions;

public static class ConfigurationExtension
{
    public static ImapConnection GetImapConnection(this IConfiguration configuration)
    {
        return new(
            configuration.GetSection("EmailServer:Host").Value.ThrowIfNull(),
            configuration.GetSection("EmailAccount:Email").Value.ThrowIfNull(),
            configuration.GetSection("EmailAccount:Password").Value.ThrowIfNull()
        );
    }
}
