namespace Spravy.Service.Services;

public class SqliteDbFileFactory : IFactory<FileInfo>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IDbFileSystem dbFileSystem;

    public SqliteDbFileFactory(
        SqliteFolderOptions sqliteFolderOptions,
        IHttpContextAccessor httpContextAccessor,
        IDbFileSystem dbFileSystem
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.httpContextAccessor = httpContextAccessor;
        this.dbFileSystem = dbFileSystem;
    }

    public Result<FileInfo> Create()
    {
        var userId = httpContextAccessor.GetUserId();
        var dataBasesFolderPath = sqliteFolderOptions.DataBasesFolder.ThrowIfNullOrWhiteSpace();
        var file = dbFileSystem.GetDbFile(Path.Combine(dataBasesFolderPath, $"{userId}.db"));

        return file.ToResult();
    }
}