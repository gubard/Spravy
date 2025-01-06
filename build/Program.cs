using _build.Builds;

namespace _build;

public static class Program
{
    static int Main(string[] args)
    {
        return args[1] switch
        {
            "publish" => Build.Execute(),
            "test" => TestBuild.Execute(),
            "desktop" => DesktopPublishSingleBuild.Execute(),
            _ => throw new($"Unknown command {args[1]}")
        };
    }
}