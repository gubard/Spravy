using Microsoft.EntityFrameworkCore;
using Spravy.ToDo.Db.Sqlite.Migrator;

await using var context = new SqliteSpravyToDoDbContext();
await context.Database.MigrateAsync();