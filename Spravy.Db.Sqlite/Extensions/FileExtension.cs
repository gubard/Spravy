namespace Spravy.Db.Sqlite.Extensions;

public static class FileExtension
{
    public static string ToSqliteConnectionString(this FileInfo file)
    {
        return $"DataSource={file}";
    }
}
