namespace Spravy.Domain.Services;

public class SpravyFileSystem : ISpravyFileSystem
{
    public DirectoryInfo GetDbDirectory()
    {
        switch (OsHelper.Os)
        {
            case Os.Windows:
                return new("D:/DataBases/");
            default: return new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DataBases"));
        }
    }
}