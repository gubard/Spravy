using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Service.HostedServices;

public class MigratorHostedService : IHostedService
{
    private readonly SqliteFileOptions sqliteFileOptions;
    private readonly IFactory<string, SpravyAuthenticationDbContext> spravyAuthenticationDbContextFactory;

    public MigratorHostedService(
        SqliteFileOptions sqliteFileOptions,
        IFactory<string, SpravyAuthenticationDbContext> spravyAuthenticationDbContextFactory
    )
    {
        this.sqliteFileOptions = sqliteFileOptions;
        this.spravyAuthenticationDbContextFactory = spravyAuthenticationDbContextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dataBaseFile = sqliteFileOptions.DataBaseFile.ThrowIfNull().ToFile();
        await using var context = spravyAuthenticationDbContextFactory.Create(dataBaseFile.ToSqliteConnectionString());

        if (dataBaseFile.Directory is not null && !dataBaseFile.Directory.Exists)
        {
            dataBaseFile.Directory.Create();
        }

        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}