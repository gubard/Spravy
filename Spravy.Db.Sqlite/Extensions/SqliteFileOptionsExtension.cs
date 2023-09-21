using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;

namespace Spravy.Db.Sqlite.Extensions;

public static class SqliteFileOptionsExtension
{
    public static string ToSqliteConnectionString(this SqliteFileOptions options)
    {
        return options.DataBaseFile.ThrowIfNull().ToFile().ToSqliteConnectionString();
    }
}