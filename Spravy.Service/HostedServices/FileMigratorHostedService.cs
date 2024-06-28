namespace Spravy.Service.HostedServices;

public class FileMigratorHostedService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly ILogger<FileMigratorHostedService<TDbContext>> logger;
    private readonly IFactory<string, TDbContext> spravyAuthenticationDbContextFactory;
    private readonly SqliteFileOptions sqliteFileOptions;

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

    public async Task StartAsync(CancellationToken ct)
    {
        var dataBaseFile = sqliteFileOptions.DataBaseFile.ThrowIfNull().ToFile();
        logger.LogInformation("Start migration {DataBaseFile}", dataBaseFile);

        await using var context = spravyAuthenticationDbContextFactory
            .Create(dataBaseFile.ToSqliteConnectionString())
            .ThrowIfError();

        if (dataBaseFile.Directory is not null && !dataBaseFile.Directory.Exists)
        {
            dataBaseFile.Directory.Create();
        }

        await context.Database.MigrateAsync(ct);
        logger.LogInformation("End migration {DataBaseFile}", dataBaseFile);
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    private string GetMigrationId()
    {
        return AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
            {
                var dbContextAttribute = x.GetCustomAttribute<DbContextAttribute>();

                if (dbContextAttribute is null)
                {
                    return false;
                }

                return dbContextAttribute.ContextType == typeof(TDbContext);
            })
            .Select(x => x.GetCustomAttribute<MigrationAttribute>())
            .Where(x => x is not null)
            .Select(x => x.ThrowIfNull().Id)
            .OrderByDescending(x => x)
            .First();
    }
}
