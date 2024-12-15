namespace Spravy.Db.Services;

public class DbFileSystem : IDbFileSystem
{
    private readonly ISpravyFileSystem spravyFileSystem;

    public DbFileSystem(ISpravyFileSystem spravyFileSystem)
    {
        this.spravyFileSystem = spravyFileSystem;
    }

    public ReadOnlyMemory<FileInfo> GetDbFiles(string path)
    {
        return GetDbDirectory(path).GetFiles("*.db");
    }

    public DirectoryInfo GetDbDirectory(string path)
    {
        return spravyFileSystem.GetDbDirectory().Combine(path);
    }

    public FileInfo GetDbFile(string path)
    {
        return spravyFileSystem.GetDbDirectory().ToFile(path);
    }
}