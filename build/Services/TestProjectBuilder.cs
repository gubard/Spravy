using System.IO;
using System.Text;
using System.Text.Json;
using _build.Extensions;
using _build.Models;
using Nuke.Common.Tools.DotNet;
using Serilog;

namespace _build.Services;

public class TestProjectBuilder : ProjectBuilder<TestProjectBuilderOptions>
{
    public TestProjectBuilder(TestProjectBuilderOptions options, VersionService versionService)
        : base(options, versionService)
    {
    }

    public override void Setup()
    {
        Log.Logger.Information("Set app settings {File}", Options.AppSettingsFile);
        var jsonDocument = Options.AppSettingsFile.GetJsonDocument();
        using var stream = new MemoryStream();
        SetAppSettingsStream(stream, jsonDocument);
        var jsonData = Encoding.UTF8.GetString(stream.ToArray());
        File.WriteAllText(Options.AppSettingsFile.FullName, jsonData);
    }

    public void Test() => DotNetTasks.DotNetTest(s =>
        s.SetConfiguration(Options.Configuration)
            .EnableNoRestore()
            .EnableNoBuild()
            .SetProjectFile(Options.CsprojFile.FullName)
    );

    void SetAppSettingsStream(Stream stream, JsonDocument jsonDocument)
    {
        var jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true
        };

        using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
        writer.WriteStartObject();

        foreach (var obj in jsonDocument.RootElement.EnumerateObject())
        {
            if (SetEmailAccount(writer, obj))
            {
                continue;
            }

            obj.WriteTo(writer);
        }

        writer.WriteEndObject();
    }

    bool SetEmailAccount(Utf8JsonWriter writer, JsonProperty property)
    {
        if (property.Name != "EmailAccount" || property.Name != "EmailAccount2")
        {
            return false;
        }

        writer.AddObject(property.Name, () =>
        {
            foreach (var obj in property.Value.EnumerateObject())
            {
                if (obj.Name == "Password")
                {
                    writer.AddStringValue("Password",
                        property.Name == "EmailAccount" ? Options.EmailAccountPassword : Options.EmailAccount2Password);
                    continue;
                }

                obj.WriteTo(writer);
            }
        });

        return true;
    }
}