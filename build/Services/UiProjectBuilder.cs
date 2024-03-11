using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;

namespace _build.Services;

public abstract class UiProjectBuilder : ProjectBuilder
{
    protected UiProjectBuilder(
        FileInfo csprojFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        VersionService versionService
    ) : base(csprojFile, hosts, runtimes, configuration, versionService)
    {
    }

    public override void Setup(string host)
    {
        var jsonDocument = appSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        stream.SetAppSettingsStream(host, jsonDocument, hosts);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(appSettingsFile.FullName, jsonData);
    }

    public override void Compile()
    {
        for (var i = 0; i < 3; i++)
        {
            try
            {
                DotNetTasks.DotNetBuild(setting =>
                    {
                        var result = setting.SetProjectFile(csprojFile.FullName)
                            .EnableNoRestore()
                            .SetConfiguration(configuration)
                            .AddProperty("Version", versionService.Version.ToString());

                        if (runtimes.IsEmpty)
                        {
                            result = result.SetRuntime(runtimes.ToArray().Select(x => x.Name).JoinSemicolon());
                        }

                        return result;
                    }
                );
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