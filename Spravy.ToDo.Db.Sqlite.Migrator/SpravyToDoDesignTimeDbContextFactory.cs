namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpravyDbToDoDbContext>
{
    public SpravyDbToDoDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new(options, new SqliteToDoDbContextSetup());
    }
}