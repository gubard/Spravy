namespace Spravy.Domain.Services;

public class SpravyFileSystem : ISpravyFileSystem
{
    public DirectoryInfo GetDbDirectory()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return new(Path.Combine(home, "DataBases"));
    }
}