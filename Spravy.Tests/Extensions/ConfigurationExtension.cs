using Microsoft.Extensions.Configuration;
using Spravy.Tests.Models;

namespace Spravy.Tests.Extensions;

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