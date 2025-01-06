using _build.Builds;

namespace _build;

public static class Program
{
    static int Main(string[] args)
    {
        var buildIndex = 0;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];

            if (arg != "--build")
            {
                continue;
            }

            buildIndex = index;

            break;
        }

        return args[buildIndex + 1] switch
        {
            "publish" => Build.Execute(),
            "test" => TestBuild.Execute(),
            "desktop" => DesktopPublishSingleBuild.Execute(),
            _ => throw new($"Unknown command {args[1]}")
        };
    }
}