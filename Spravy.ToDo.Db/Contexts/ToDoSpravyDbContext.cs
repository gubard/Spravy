namespace Spravy.ToDo.Db.Contexts;

public class ToDoSpravyDbContext : SpravyDbContext, IDbContextCreator<ToDoSpravyDbContext>
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

    protected ToDoSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public ToDoSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static ToDoSpravyDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}