namespace Spravy.Domain.Helpers;

public static class AppConst
{
    public static readonly Guid AppInstanceId;
    public static readonly SpravyVersion Version;
    public static readonly string VersionString;

    static AppConst()
    {
        var versionStr = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        AppInstanceId = Guid.NewGuid();

        if (!SpravyVersion.TryParse(versionStr, out Version))
        {
            Version = new(1, 0, 0, 0);
        }

        VersionString = $"{Version}({Version.Code})";
    }
}