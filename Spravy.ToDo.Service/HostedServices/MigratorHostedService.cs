using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Service.Models;

namespace Spravy.ToDo.Service.HostedServices;

public class MigratorHostedService : IHostedService
{
    private const string MigrationFileName = ".migration";
    private readonly SqliteOptions sqliteOptions;
    private readonly IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory;

    public MigratorHostedService(
        SqliteOptions sqliteOptions,
        IFactory<string, SpravyToDoDbContext> spravyToDoDbContextFactory
    )
    {
        this.sqliteOptions = sqliteOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var migrationFile = sqliteOptions.DataBasesFolder.ToDirectory().ToFile(MigrationFileName);
        var migrationId = GetMigrationId();

        if (!await IsNeedMigration(migrationFile, migrationId))
        {
            return;
        }

        var dataBaseFiles = sqliteOptions.DataBasesFolder.ToDirectory().GetFiles("*.db");

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