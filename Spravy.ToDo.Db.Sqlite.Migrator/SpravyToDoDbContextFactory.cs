namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDbContextFactory : IFactory<string, SpravyDbToDoDbContext>
{
    private readonly IDbContextSetup dbContextSetup;
    private readonly ILoggerFactory loggerFactory;

    public SpravyToDoDbContextFactory(IDbContextSetup dbContextSetup, ILoggerFactory loggerFactory)
    {
        this.dbContextSetup = dbContextSetup;
        this.loggerFactory = loggerFactory;
    }

    public Result<SpravyDbToDoDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName)
            )
            .UseLoggerFactory(loggerFactory)
            .Options;

        var context = new SpravyDbToDoDbContext(options, dbContextSetup);

        return context.ToResult();
    }
}
