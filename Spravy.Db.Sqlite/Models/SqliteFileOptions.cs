using Spravy.Domain.Interfaces;

namespace Spravy.Db.Sqlite.Models;

public class SqliteFileOptions : IOptionsValue
{
    public static string Section => "Sqlite";

    public string? DataBaseFile { get; set; }
}