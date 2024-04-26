using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Service.Services;

public class SqliteDbContextFactory<TDbContext> : IFactory<TDbContext>
    where TDbContext : DbContext, IDbContextCreator<TDbContext>
{
    private readonly IFactory<FileInfo> fileFactory;
    private readonly IFactory<string, TDbContext> dbContextFactory;

    public SqliteDbContextFactory(
        IFactory<string, TDbContext> dbContextFactory,
        IFactory<FileInfo> fileFactory
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.fileFactory = fileFactory;
    }

    public Result<TDbContext> Create()
    {
        return fileFactory.Create().IfSuccess(connectionString =>
            dbContextFactory.Create(connectionString.ToSqliteConnectionString()));
    }
}