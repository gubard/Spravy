using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Nuke.Common.Tools.DotNet;
using Serilog;
using _build.Extensions;
using _build.Models;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder<TestProjectBuilderOptions>
{
    public TestProjectBuilder(TestProjectBuilderOptions options, VersionService versionService)
        : base(options, versionService) { }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", Options.AppSettingsFile);
        var jsonDocument = Options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        SetAppSettingsStream(stream, jsonDocument);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Options.AppSettingsFile.FullName, jsonData);
    }

    public void Test()
    {
        for (var i = 0ul; i < ulong.MaxValue; i++)
        {
            var output = DotNetTasks.DotNetTest(s =>
                s.SetConfiguration(Options.Configuration)
                    .EnableNoRestore()
                    .EnableNoBuild()
                    .SetProjectFile(Options.CsprojFile.FullName)
                    .SetFilter($"Priority={i}")
            );

            if (output.Any(x => x.Text.Contains("No test matches the given testcase filter")))
            {
                break;
            }
        }
    }

    void SetAppSettingsStream(Stream stream, JsonDocument jsonDocument)
    {
        var jsonWriterOptions = new JsonWriterOptions { Indented = true, };

        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
        writer.WriteStartObject();

        foreach (var obj in jsonDocument.RootElement.EnumerateObject())
        {
            if (SetEmailAccount(writer, obj))
            {
                continue;
            }

            if (writer.SetServices(obj, Options.Domain, Options.Hosts, string.Empty))
            {
                continue;
            }

            obj.WriteTo(writer);
        }

        writer.WriteEndObject();
    }

    bool SetEmailAccount(Utf8JsonWriter writer, JsonProperty property)
    {
        if (property.Name != "EmailAccount" && property.Name != "EmailAccount2")
        {
            return false;
        }

        writer.AddObject(
            property.Name,
            () =>
            {
                foreach (var obj in property.Value.EnumerateObject())
                {
                    if (obj.Name == "Password")
                    {
                        Log.Information("Set password for {Name}", property.Name);

                        writer.AddStringValue(
                            "Password",
                            property.Name == "EmailAccount"
                                ? Options.EmailAccountPassword
                                : Options.EmailAccount2Password
                        );

                        continue;
                    }

                    obj.WriteTo(writer);
                }
            }
        );

        return true;
    }
}
