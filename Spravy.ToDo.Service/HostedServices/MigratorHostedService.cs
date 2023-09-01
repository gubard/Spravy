using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Db.Contexts;

namespace Spravy.ToDo.Service.HostedServices;

public class MigratorHostedService : IHostedService
{
    private const string MigrationFileName = ".migration";
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory;

    public MigratorHostedService(
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var migrationFile = sqliteFolderOptions.DataBasesFolder.ToDirectory().ToFile(MigrationFileName);
        var migrationId = GetMigrationId();

        if (!await IsNeedMigration(migrationFile, migrationId))
        {
            return;
        }

        var dataBaseFiles = sqliteFolderOptions.DataBasesFolder.ToDirectory().GetFiles("*.db");

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

                    return dbContextAttribute.ContextType == typeof(SpravyToDoDbContext);
                }
            )
            .Select(x => x.GetCustomAttribute<MigrationAttribute>())
            .Where(x => x is not null)
            .Select(x => x.Id)
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