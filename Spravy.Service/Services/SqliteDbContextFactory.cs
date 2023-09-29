using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.Service.Services;

public class SqliteDbContextFactory<TDbContext> : IFactory<TDbContext>
    where TDbContext : DbContext, IDbContextCreator<TDbContext>
{
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IFactory<string, TDbContext> dbContextFactory;

    public SqliteDbContextFactory(
        SqliteFolderOptions sqliteFolderOptions,
        IHttpContextAccessor httpContextAccessor,
        IFactory<string, TDbContext> dbContextFactory
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContextFactory = dbContextFactory;
    }

    public TDbContext Create()
    {
        var userId = httpContextAccessor.GetUserId();
        var fileName = $"{userId}.db";
        var file = sqliteFolderOptions.DataBasesFolder.ThrowIfNull().ToDirectory().ToFile(fileName);
        var connectionString = file.ToSqliteConnectionString();
        var dbContext = dbContextFactory.Create(connectionString);

        return dbContext;
    }
}