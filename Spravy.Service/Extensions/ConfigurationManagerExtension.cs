using Microsoft.Extensions.Configuration;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class ConfigurationManagerExtension
{
    public static ConfigurationManager AddSpravy(this ConfigurationManager manager, string[] args)
    {
        manager.Sources.Clear();
        manager.AddJsonFile(FileNames.DefaultConfigFileName);
        manager.AddEnvironmentVariables();
        manager.AddCommandLine(args);

        return manager;
    }
}