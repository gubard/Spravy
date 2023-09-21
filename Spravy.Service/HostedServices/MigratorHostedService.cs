using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Service.HostedServices;

public class MigratorHostedService<TDbContext> : IHostedService where TDbContext : DbContext
{
    private const string MigrationFileName = ".migration";
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, TDbContext> spravyToDoDbContextFactory;

    public MigratorHostedService(
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> spravyToDoDbContextFactory
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var migrationFile = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace()
            .ToDirectory()
            .ToFile(MigrationFileName);

        var migrationId = GetMigrationId();

        if (!await IsNeedMigration(migrationFile, migrationId))
        {
            return;
        }

        var dataBasesFolder = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace().ToDirectory();

        if (!dataBasesFolder.Exists)
        {
            dataBasesFolder.Create();
        }

        var dataBaseFiles = dataBasesFolder.GetFiles("*.db");

        foreach (var dataBaseFile in dataBaseFiles)
        {
            await using var spravyToDoDbContext = spravyToDoDbContextFactory.Create($"DataSource={dataBaseFile}");
            await spravyToDoDbContext.Database.MigrateAsync(cancellationToken);
        }

        if (migrationFile.Exists)
        {
            migrationFile.Delete();
        }

        await migrationFile.WriteAllTextAsync(migrationId);
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