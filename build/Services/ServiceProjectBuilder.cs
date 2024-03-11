using System.IO;
using System.Text;
using _build.Extensions;
using _build.Models;
using Serilog;

namespace _build.Services;

public class ServiceProjectBuilder : ProjectBuilder
{
    readonly ServiceProjectBuilderOptions serviceProjectBuilderOptions;

    public ServiceProjectBuilder(
        ProjectBuilderOptions projectBuilderOptions,
        VersionService versionService,
        ServiceProjectBuilderOptions serviceProjectBuilderOptions
    ) : base(projectBuilderOptions, versionService)
    {
        this.serviceProjectBuilderOptions = serviceProjectBuilderOptions;
    }

    public override void Setup(string host)
    {
        var jsonDocument = projectBuilderOptions.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        Log.Logger.Information("Set app settings {File}", projectBuilderOptions.AppSettingsFile);

        stream.SetAppSettingsStream(
            jsonDocument,
            host,
            projectBuilderOptions.Hosts,
            serviceProjectBuilderOptions.Token,
            serviceProjectBuilderOptions.Port,
            serviceProjectBuilderOptions.EmailPassword
        );

        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(projectBuilderOptions.AppSettingsFile.FullName, jsonData);
    }
}