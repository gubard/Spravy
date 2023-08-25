using Microsoft.EntityFrameworkCore;

namespace Spravy.Db.Interfaces;

public interface IDbContextSetup
{
    void OnModelCreating(ModelBuilder modelBuilder);
}