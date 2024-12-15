namespace Spravy.Service.HostedServices;

public class FolderMigratorHostedService<TDbContext> : IHostedService where TDbContext : DbContext
{
    private const string MigrationFileName = ".migration";
    private readonly IFactory<string, TDbContext> dbContextFactory;
    private readonly ILogger<FolderMigratorHostedService<TDbContext>> logger;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IDbFileSystem dbFileSystem;

    public FolderMigratorHostedService(
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> dbContextFactory,
        ILogger<FolderMigratorHostedService<TDbContext>> logger,
        IDbFileSystem dbFileSystem
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.dbFileSystem = dbFileSystem;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var dbDirectory = dbFileSystem.GetDbDirectory(sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace());
        var migrationFile = dbDirectory.ToFile(MigrationFileName);

        var migrationId = GetMigrationId();
        logger.LogInformation("Start migration to {MigrationId}", migrationId);

        if (!await IsNeedMigration(migrationFile, migrationId))
        {
            logger.LogInformation("End migration to {MigrationId}", migrationId);

            return;
        }

        if (!dbDirectory.Exists)
        {
            dbDirectory.Create();
        }

        var dataBaseFiles = dbFileSystem.GetDbFiles(sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace());

        foreach (var dataBaseFile in dataBaseFiles.ToArray())
        {
            logger.LogInformation("Start migration {MigrationId} {DataBaseFile}", migrationId, dataBaseFile);
            await using var spravyToDoDbContext = dbContextFactory.Create($"DataSource={dataBaseFile}").ThrowIfError();
            await spravyToDoDbContext.Database.MigrateAsync(ct);
            logger.LogInformation("End migration {MigrationId} {DataBaseFile}", migrationId, dataBaseFile);
        }

        if (migrationFile.Exists)
        {
            migrationFile.Delete();
        }

        await migrationFile.WriteAllTextAsync(migrationId);
        logger.LogInformation("End migration to {MigrationId}", migrationId);
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    private string GetMigrationId()
    {
        return AppDomain.CurrentDomain
           .GetAssemblies()
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