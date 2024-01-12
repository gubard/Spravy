using Spravy.Domain.Extensions;

namespace Spravy.Tests.Helpers;

public static class FileHelper
{
    public static FileInfo GetFrameShortFile()
    {
        return Path.GetTempPath().ToDirectory().Combine("Spravy").ToFile($"{DateTime.Now:yyyy-MM-dd-hh-mm-ss}-{Guid.NewGuid()}.png");
    }
}