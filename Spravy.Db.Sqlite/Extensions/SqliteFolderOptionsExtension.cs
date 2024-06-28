namespace Spravy.Db.Sqlite.Extensions;

public static class SqliteFolderOptionsExtension
{
    public static FileInfo[] GetDataBaseFiles(this SqliteFolderOptions options)
    {
        var folder = options.DataBasesFolder.ThrowIfNull().ToDirectory();

        return folder.GetFiles("*.db");
    }

    public static IEnumerable<string> GetConnectionStrings(this SqliteFolderOptions options)
    {
        var dataBaseFiles = options.GetDataBaseFiles();

        foreach (var dataBaseFile in dataBaseFiles)
        {
            yield return dataBaseFile.ToSqliteConnectionString();
        }
    }
}
