using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.Service.Middlewares;

public class DataBaseSetupSqliteMiddleware<TDbContext> where TDbContext : DbContext
{
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, TDbContext> spravyToDoDbContextFactory;
    private readonly RequestDelegate next;

    public DataBaseSetupSqliteMiddleware(
        RequestDelegate next,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> spravyToDoDbContextFactory
    )
    {
        this.next = next;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.GetUserId();
        await SetupDataBaseAsync(userId);
        await next.Invoke(context);
    }

    private async Task SetupDataBaseAsync(string userId)
    {
        var dataBaseFile = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace()
            .ToDirectory()
            .ToFile($"{userId}.db");

        if (dataBaseFile.Exists)
        {
            return;
        }

        await using var spravyToDoDbContext = spravyToDoDbContextFactory.Create($"DataSource={dataBaseFile}");
        await spravyToDoDbContext.Database.MigrateAsync();
    }
}