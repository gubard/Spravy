using System;
using System.IO;
using System.Text;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public abstract class UiProjectBuilder<TOptions> : ProjectBuilder<TOptions> where TOptions : ProjectBuilderOptions
{
    protected UiProjectBuilder(TOptions options, SpravyVersion version) : base(options, version)
    {
    }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", Options.AppSettingsFile);
        var jsonDocument = Options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(Options.Domain, jsonDocument, Options.Hosts);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Options.AppSettingsFile.FullName, jsonData);
    }

    public override void Compile()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (Options.Runtimes.IsEmpty)
                {
                    DotNetTasks.DotNetBuild(
                        setting => setting.SetProjectFile(Options.CsprojFile.FullName)
                           .EnableNoRestore()
                           .SetConfiguration(Options.Configuration)
                           .AddProperty("Version", version.ToString())
                    );
                }
                else
                {
                    foreach (var runtime in Options.Runtimes.Span)
                    {
                        DotNetTasks.DotNetBuild(
                            setting => setting.SetProjectFile(Options.CsprojFile.FullName)
                               .EnableNoRestore()
                               .SetConfiguration(Options.Configuration)
                               .AddProperty("Version", version.ToString())
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