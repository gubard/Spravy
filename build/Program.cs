using System;
using _build.Builds;
using Serilog;

namespace _build;

public static class Program
{
    static int Main(string[] args)
    {
        var buildIndex = 0;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            Console.WriteLine($"Argument {arg}");

            if (arg != "--build")
            {
                continue;
            }

            buildIndex = index + 1;

            break;
        }

        Console.WriteLine($"Build {args[buildIndex]}");
        
        return args[buildIndex] switch
        {
            "publish" => Build.Execute(),
            "test" => TestBuild.Execute(),
            "desktop" => DesktopPublishSingleBuild.Execute(),
            _ => throw new($"Unknown command {args[1]}")
        };
    }
}