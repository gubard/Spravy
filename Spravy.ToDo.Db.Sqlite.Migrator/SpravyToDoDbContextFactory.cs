namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDbContextFactory : IFactory<string, ToDoSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;
    private readonly ILoggerFactory loggerFactory;

    public SpravyToDoDbContextFactory(IDbContextSetup dbContextSetup, ILoggerFactory loggerFactory)
    {
        this.dbContextSetup = dbContextSetup;
        this.loggerFactory = loggerFactory;
    }

    public Result<ToDoSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
           .UseSqlite(key, b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName))
           .UseLoggerFactory(loggerFactory)
           .Options;

        var context = new ToDoSpravyDbContext(options, dbContextSetup);

        return context.ToResult();
    }
}