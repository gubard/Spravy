using Microsoft.EntityFrameworkCore;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Db.Contexts;

namespace Spravy.Schedule.Service.HostedServices;

public class MigratorHostedService : IHostedService
{
    private readonly SqliteFileOptions sqliteFileOptions;
    private readonly IFactory<string, SpravyScheduleDbContext> spravyScheduleDbContextFactory;

    public MigratorHostedService(
        SqliteFileOptions sqliteFileOptions,
        IFactory<string, SpravyScheduleDbContext> spravyScheduleDbContextFactory
    )
    {
        this.sqliteFileOptions = sqliteFileOptions;
        this.spravyScheduleDbContextFactory = spravyScheduleDbContextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dataBaseFile = sqliteFileOptions.DataBaseFile.ThrowIfNull().ToFile();
        await using var context = spravyScheduleDbContextFactory.Create($"DataSource={dataBaseFile}");

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