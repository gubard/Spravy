namespace Spravy.Db.Sqlite.Models;

public class SqliteFolderOptions : IOptionsValue
{
    public string? DataBasesFolder { get; set; }

    public static string Section
    {
        get => "Sqlite";
    }
}