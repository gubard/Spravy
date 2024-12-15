namespace Spravy.Domain.Helpers;

public static class OsHelper
{
    public static Os Os;

    static OsHelper()
    {
        Os = GetOs();
    }

    private static Os GetOs()
    {
        if (OperatingSystem.IsAndroid())
        {
            return Os.Android;
        }

        if (OperatingSystem.IsBrowser())
        {
            return Os.Browser;
        }

        if (OperatingSystem.IsLinux())
        {
            return Os.Linux;
        }

        if (OperatingSystem.IsWasi())
        {
            return Os.Wasi;
        }

        if (OperatingSystem.IsWindows())
        {
            return Os.Windows;
        }

        if (OperatingSystem.IsMacCatalyst())
        {
            return Os.MacCatalyst;
        }

        if (OperatingSystem.IsIOS())
        {
            return Os.IOs;
        }

        if (OperatingSystem.IsMacOS())
        {
            return Os.MacOs;
        }

        if (OperatingSystem.IsTvOS())
        {
            return Os.TvOs;
        }

        if (OperatingSystem.IsWatchOS())
        {
            return Os.WatchOs;
        }

        if (OperatingSystem.IsFreeBSD())
        {
            return Os.FreeBsd;
        }

        throw new ArgumentOutOfRangeException();
    }
}