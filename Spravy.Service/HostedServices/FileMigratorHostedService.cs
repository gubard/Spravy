namespace Spravy.Service.HostedServices;

public class FileMigratorHostedService<TDbContext> : IHostedService where TDbContext : DbContext
{
    private readonly ILogger<FileMigratorHostedService<TDbContext>> logger;
    private readonly IFactory<string, TDbContext> spravyAuthenticationDbContextFactory;
    private readonly SqliteFileOptions sqliteFileOptions;
    private readonly IDbFileSystem dbFileSystem;

    public FileMigratorHostedService(
        SqliteFileOptions sqliteFileOptions,
        IFactory<string, TDbContext> spravyAuthenticationDbContextFactory,
        ILogger<FileMigratorHostedService<TDbContext>> logger,
        IDbFileSystem dbFileSystem
    )
    {
        this.sqliteFileOptions = sqliteFileOptions;
        this.spravyAuthenticationDbContextFactory = spravyAuthenticationDbContextFactory;
        this.logger = logger;
        this.dbFileSystem = dbFileSystem;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var dataBaseFile = dbFileSystem.GetDbFile(sqliteFileOptions.DataBaseFile.ThrowIfNullOrWhiteSpace());
        logger.LogInformation("Start migration {DataBaseFile}", dataBaseFile);

        await using var context = spravyAuthenticationDbContextFactory.Create(dataBaseFile.ToSqliteConnectionString())
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
}