using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Service.HostedServices;

public class FileMigratorHostedService<TDbContext> : IHostedService where TDbContext : DbContext
{
    private readonly ILogger<FileMigratorHostedService<TDbContext>> logger;
    private readonly SqliteFileOptions sqliteFileOptions;
    private readonly IFactory<string, TDbContext> spravyAuthenticationDbContextFactory;

    public FileMigratorHostedService(
        SqliteFileOptions sqliteFileOptions,
        IFactory<string, TDbContext> spravyAuthenticationDbContextFactory,
        ILogger<FileMigratorHostedService<TDbContext>> logger
    )
    {
        this.sqliteFileOptions = sqliteFileOptions;
        this.spravyAuthenticationDbContextFactory = spravyAuthenticationDbContextFactory;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dataBaseFile = sqliteFileOptions.DataBaseFile.ThrowIfNull().ToFile();
        logger.LogInformation("Start migration {DataBaseFile}", dataBaseFile);
        await using var context = spravyAuthenticationDbContextFactory.Create(dataBaseFile.ToSqliteConnectionString());

        if (dataBaseFile.Directory is not null && !dataBaseFile.Directory.Exists)
        {
            dataBaseFile.Directory.Create();
        }

        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("End migration {DataBaseFile}", dataBaseFile);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private string GetMigrationId()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(
                x =>
                {
                    var dbContextAttribute = x.GetCustomAttribute<DbContextAttribute>();

                    if (dbContextAttribute is null)
                    {
                        return false;
                    }

                    return dbContextAttribute.ContextType == typeof(TDbContext);
                }
            )
            .Select(x => x.GetCustomAttribute<MigrationAttribute>())
            .Where(x => x is not null)
            .Select(x => x.ThrowIfNull().Id)
            .OrderByDescending(x => x)
            .First();
    }
}