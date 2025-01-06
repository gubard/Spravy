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
            Log.Logger.Information("Argument {Arg}", arg);

            if (arg != "--build")
            {
                continue;
            }

            buildIndex = index;

            break;
        }

        Log.Logger.Information("Build {Arg}", buildIndex + 1);
        
        return args[buildIndex + 1] switch
        {
            "publish" => Build.Execute(),
            "test" => TestBuild.Execute(),
            "desktop" => DesktopPublishSingleBuild.Execute(),
            _ => throw new($"Unknown command {args[1]}")
        };
    }
}