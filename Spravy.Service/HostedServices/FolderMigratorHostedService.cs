using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Service.HostedServices;

public class FolderMigratorHostedService<TDbContext> : IHostedService where TDbContext : DbContext
{
    private const string MigrationFileName = ".migration";
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, TDbContext> dbContextFactory;
    private readonly ILogger<FolderMigratorHostedService<TDbContext>> logger;

    public FolderMigratorHostedService(
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> dbContextFactory,
        ILogger<FolderMigratorHostedService<TDbContext>> logger
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var migrationFile = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace()
            .ToDirectory()
            .ToFile(MigrationFileName);

        cancellationToken.ThrowIfCancellationRequested();
        var migrationId = GetMigrationId();

        logger.LogInformation("Start migration to {MigrationId}", migrationId);

        if (!await IsNeedMigration(migrationFile, migrationId))
        {
            logger.LogInformation("End migration to {MigrationId}", migrationId);

            return;
        }

        var dataBasesFolder = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace().ToDirectory();

        if (!dataBasesFolder.Exists)
        {
            dataBasesFolder.Create();
        }

        var dataBaseFiles = dataBasesFolder.GetFiles("*.db");
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var dataBaseFile in dataBaseFiles)
        {
            logger.LogInformation("Start migration {MigrationId} {DataBaseFile}", migrationId, dataBaseFile);
            await using var spravyToDoDbContext = dbContextFactory.Create($"DataSource={dataBaseFile}");
            await spravyToDoDbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("End migration {MigrationId} {DataBaseFile}", migrationId, dataBaseFile);
        }

        if (migrationFile.Exists)
        {
            migrationFile.Delete();
        }

        await migrationFile.WriteAllTextAsync(migrationId);
        logger.LogInformation("End migration to {MigrationId}", migrationId);
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

    private async Task<bool> IsNeedMigration(FileInfo migrationFile, string migrationId)
    {
        if (!migrationFile.Exists)
        {
            return true;
        }

        var text = await migrationFile.ReadAllTextAsync();

        return migrationId != text;
    }
}