namespace Spravy.Integration.Tests.Helpers;

public static class FileHelper
{
    public static FileInfo GetFrameShortFile()
    {
        var file = Path.GetTempPath()
           .ToDirectory()
           .Combine("spravy")
           .ToFile($"{DateTime.Now:yyyy-MM-dd-hh-mm-ss}-{Guid.NewGuid()}.png");

        return file;
    }
}