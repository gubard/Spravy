namespace Spravy.Db.Contexts;

public abstract class SpravyDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyDbContext(IDbContextSetup setup)
    {
        this.setup = setup;

        if (setup is { AutoCreateDataBase: true, DataBaseCreated: false, })
        {
            Database.EnsureCreated();
        }
    }

    protected SpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    public bool IsDisposed { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        setup.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        setup.OnConfiguring(optionsBuilder);
    }

    public override void Dispose()
    {
        IsDisposed = true;
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        IsDisposed = true;
        await base.DisposeAsync();
    }
}