using Microsoft.EntityFrameworkCore;

namespace Spravy.Db.Interfaces;

public interface IDbContextCreator<out TContext> where TContext : DbContext
{
    static abstract TContext CreateContext(IDbContextSetup setup, DbContextOptions options);
}