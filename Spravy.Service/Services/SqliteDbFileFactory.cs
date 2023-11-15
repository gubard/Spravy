using Microsoft.AspNetCore.Http;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.Service.Services;

public class SqliteDbFileFactory : IFactory<FileInfo>
{
    private readonly SqliteFolderOptions sqliteFolderOptions;
    private readonly IHttpContextAccessor httpContextAccessor;

    public SqliteDbFileFactory(SqliteFolderOptions sqliteFolderOptions, IHttpContextAccessor httpContextAccessor)
    {
        this.sqliteFolderOptions = sqliteFolderOptions;
        this.httpContextAccessor = httpContextAccessor;
    }

    public FileInfo Create()
    {
        var userId = httpContextAccessor.GetUserId();
        var fileName = $"{userId}.db";
        var file = sqliteFolderOptions.DataBasesFolder.ThrowIfNull().ToDirectory().ToFile(fileName);

        return file;
    }
}