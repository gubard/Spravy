using System;
using System.IO;
using System.Text;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;

namespace _build.Services;

public abstract class UiProjectBuilder : ProjectBuilder
{
    protected UiProjectBuilder(ProjectBuilderOptions projectBuilderOptions, VersionService versionService)
        : base(projectBuilderOptions, versionService)
    {
    }

    public override void Setup(string host)
    {
        var jsonDocument = projectBuilderOptions.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(host, jsonDocument, projectBuilderOptions.Hosts);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(projectBuilderOptions.AppSettingsFile.FullName, jsonData);
    }

    public override void Compile()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (projectBuilderOptions.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetBuild(setting => setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                        .EnableNoRestore()
                        .SetConfiguration(projectBuilderOptions.Configuration)
                        .AddProperty("Version", versionService.Version.ToString())
                    );
                }
                else
                {
                    foreach (var runtime in projectBuilderOptions.Runtimes.Span)
                    {
                        DotNetTasks.DotNetBuild(setting =>
                            setting.SetProjectFile(projectBuilderOptions.CsprojFile.FullName)
                                .EnableNoRestore()
                                .SetConfiguration(projectBuilderOptions.Configuration)
                                .AddProperty("Version", versionService.Version.ToString())
                                .SetRuntime(runtime.Name)
                        );
                    }
                }
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