using Microsoft.EntityFrameworkCore;
using Spravy.ToDo.Db.Sqlite.Migrator;

await using var context = new SqliteSpravyDbContext();
await context.Database.MigrateAsync();