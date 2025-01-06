using System;
using _build.Builds;
using _build.Enums;

namespace _build;

public static class Program
{
    static int Main(string[] args)
    {
        var buildType = BuildType.Publish;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];

            if (arg == "--build-desktop")
            {
                buildType = BuildType.Desktop;

                break;
            }

            if (arg == "--build-test")
            {
                buildType = BuildType.Test;

                break;
            }

            if (arg == "--build-publish")
            {
                buildType = BuildType.Publish;

                break;
            }
        }

        return buildType switch
        {
            BuildType.Publish => Build.Execute(),
            BuildType.Test => TestBuild.Execute(),
            BuildType.Desktop => DesktopPublishSingleBuild.Execute(),
            _ => throw new($"Unknown command {args[1]}"),
        };
    }
}