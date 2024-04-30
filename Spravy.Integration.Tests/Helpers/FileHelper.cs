using Spravy.Domain.Extensions;

namespace Spravy.Integration.Tests.Helpers;

public static class FileHelper
{
    public static FileInfo GetFrameShortFile()
    {
        return Path.GetTempPath()
           .ToDirectory()
           .Combine("spravy")
           .ToFile($"{DateTime.Now:yyyy-MM-dd-hh-mm-ss}-{Guid.NewGuid()}.png");
    }
}