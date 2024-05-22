namespace Spravy.Db.Sqlite.Models;

public class SqliteFileOptions : IOptionsValue
{
    public string? DataBaseFile { get; set; }

    public static string Section
    {
        get => "Sqlite";
    }
}