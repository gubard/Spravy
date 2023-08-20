using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Sqlite.Migrator;

await using var context = new SqliteSpravyAuthenticationDbContext();
await context.Database.MigrateAsync();