namespace Spravy.Schedule.Db.Contexts;

public class ScheduleSpravyDbContext : SpravyDbContext, IDbContextCreator<ScheduleSpravyDbContext>
{
    protected ScheduleSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public ScheduleSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static ScheduleSpravyDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}