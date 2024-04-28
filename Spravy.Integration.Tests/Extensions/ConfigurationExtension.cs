using Microsoft.Extensions.Configuration;
using Spravy.Integration.Tests.Models;

namespace Spravy.Integration.Tests.Extensions;

public static class ConfigurationExtension
{
    public static ImapConnection GetImapConnection(this IConfiguration configuration)
    {
        return new ImapConnection(
            configuration.GetSection("EmailServer:Host").Value,
            configuration.GetSection("EmailAccount:Email").Value,
            configuration.GetSection("EmailAccount:Password").Value
        );
    }
}