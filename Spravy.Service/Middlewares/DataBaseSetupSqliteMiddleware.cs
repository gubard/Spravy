namespace Spravy.Service.Middlewares;

public class DataBaseSetupSqliteMiddleware<TDbContext> where TDbContext : DbContext
{
    private readonly RequestDelegate next;
    private readonly IFactory<string, TDbContext> spravyToDoDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IDbFileSystem dbFileSystem;

    public DataBaseSetupSqliteMiddleware(
        RequestDelegate next,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> spravyToDoDbContextFactory,
        IDbFileSystem dbFileSystem
    )
    {
        this.next = next;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
        this.dbFileSystem = dbFileSystem;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.GetUserId();
        await SetupDataBaseAsync(userId);
        await next.Invoke(context);
    }

    private async Task SetupDataBaseAsync(string userId)
    {
        var dataBasesFolderPath = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace();
        var dataBaseFile = dbFileSystem.GetDbFile(Path.Combine(dataBasesFolderPath, $"{userId}.db"));

        if (dataBaseFile.Exists)
        {
            return;
        }

        if (dataBaseFile.Directory is not null && !dataBaseFile.Directory.Exists)
        {
            dataBaseFile.Directory.Create();
        }

        await using var dbContext = spravyToDoDbContextFactory.Create($"DataSource={dataBaseFile}").ThrowIfError();
        await dbContext.Database.MigrateAsync();
    }
}