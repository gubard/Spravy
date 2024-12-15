namespace Spravy.Db.Interfaces;

public interface IDbFileSystem
{
    ReadOnlyMemory<FileInfo> GetDbFiles(string path);
    DirectoryInfo GetDbDirectory(string path);
    FileInfo GetDbFile(string path);
}