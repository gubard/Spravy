namespace Spravy.Service.Services;

public class SqliteDbContextFactory<TDbContext> : IFactory<TDbContext>
    where TDbContext : DbContext, IDbContextCreator<TDbContext>
{
    private readonly IFactory<string, TDbContext> dbContextFactory;
    private readonly IFactory<FileInfo> fileFactory;

    public SqliteDbContextFactory(
        IFactory<string, TDbContext> dbContextFactory,
        IFactory<FileInfo> fileFactory
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.fileFactory = fileFactory;
    }

    public Result<TDbContext> Create()
    {
        return fileFactory
            .Create()
            .IfSuccess(connectionString =>
                dbContextFactory.Create(connectionString.ToSqliteConnectionString())
            );
    }
}
