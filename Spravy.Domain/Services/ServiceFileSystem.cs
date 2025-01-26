namespace Spravy.Domain.Services;

public class SpravyFileSystem : ISpravyFileSystem
{
    private static readonly string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly DirectoryInfo DbDirectory = new(Path.Combine(UserProfile, "DataBases"));
    private static readonly DirectoryInfo FilesDirectory = new(Path.Combine(UserProfile, "Files"));

    public DirectoryInfo GetDbDirectory()
    {
        return DbDirectory;
    }

    public DirectoryInfo GetFilesDirectory()
    {
        return FilesDirectory;
    }
}