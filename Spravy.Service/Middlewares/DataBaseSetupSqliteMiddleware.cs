namespace Spravy.Service.Middlewares;

public class DataBaseSetupSqliteMiddleware<TDbContext> where TDbContext : DbContext
{
    private readonly RequestDelegate next;
    private readonly IFactory<string, TDbContext> spravyToDoDbContextFactory;
    private readonly SqliteFolderOptions sqliteFolderOptions;

    public DataBaseSetupSqliteMiddleware(
        RequestDelegate next,
        SqliteFolderOptions sqliteFolderOptions,
        IFactory<string, TDbContext> spravyToDoDbContextFactory
    )
    {
        this.next = next;
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.spravyToDoDbContextFactory = spravyToDoDbContextFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.GetUserId();
        await SetupDataBaseAsync(userId);
        await next.Invoke(context);
    }

    private async Task SetupDataBaseAsync(string userId)
    {
        var dataBaseFile = sqliteFolderOptions.DataBasesFolder
           .ThrowIfNullOrWhiteSpace()
           .ToDirectory()
           .ToFile($"{userId}.db");

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