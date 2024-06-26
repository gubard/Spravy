namespace Spravy.Service.Services;

public class SqliteDbFileFactory : IFactory<FileInfo>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly SqliteFolderOptions sqliteFolderOptions;

    public SqliteDbFileFactory(
        SqliteFolderOptions sqliteFolderOptions,
        IHttpContextAccessor httpContextAccessor
    )
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.httpContextAccessor = httpContextAccessor;
    }

    public Result<FileInfo> Create()
    {
        var userId = httpContextAccessor.GetUserId();
        var fileName = $"{userId}.db";
        var file = sqliteFolderOptions.DataBasesFolder.ThrowIfNull().ToDirectory().ToFile(fileName);

        return file.ToResult();
    }
}
