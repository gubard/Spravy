using System;
using System.IO;
using System.Text;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public abstract class UiProjectBuilder : ProjectBuilder
{
    protected UiProjectBuilder(ProjectBuilderOptions options, VersionService versionService)
        : base(options, versionService)
    {
    }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", options.AppSettingsFile);
        var jsonDocument = options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(options.Domain, jsonDocument, options.Hosts);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(options.AppSettingsFile.FullName, jsonData);
    }

    public override void Compile()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (options.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(options.CsprojFile.FullName)
                        .EnableNoRestore()
                        .SetConfiguration(options.Configuration)
                        .AddProperty("Version", versionService.Version.ToString())
                    );
                }
                else
                {
                    foreach (var runtime in options.Runtimes.Span)
                    {
                        DotNetTasks.DotNetBuild(setting =>
                            setting.SetProjectFile(options.CsprojFile.FullName)
                                .EnableNoRestore()
                                .SetConfiguration(options.Configuration)
                                .AddProperty("Version", versionService.Version.ToString())
                                .SetRuntime(runtime.Name)
                        );
                    }
                }

                break;
            }
            catch (Exception e)
            {
                if (i == 2)
                {
                    throw;
                }

                if (e.ToString().Contains("CompileAvaloniaXamlTask"))
                {
                    continue;
                }

                throw;
            }
        }
    }
}