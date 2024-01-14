using Microsoft.Extensions.Configuration;
using Spravy.Domain.Helpers;

namespace Spravy.Service.Extensions;

public static class ConfigurationManagerExtension
{
    public static ConfigurationManager AddSpravy(this ConfigurationManager manager, string[] args)
    {
        manager.Sources.Clear();
        manager.AddJsonFile(FileNames.DefaultConfigFileName);
        manager.AddEnvironmentVariables("Spravy_");
        manager.AddCommandLine(args);

        return manager;
    }
}