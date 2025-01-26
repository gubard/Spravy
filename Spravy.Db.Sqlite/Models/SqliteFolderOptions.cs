namespace Spravy.Db.Sqlite.Models;

public class SqliteFolderOptions : IOptionsValue
{
    public static string Section => "Sqlite";

    public string? DataBasesFolder { get; set; }
}