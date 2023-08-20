using Microsoft.EntityFrameworkCore;

namespace Spravy.Db.Core.Interfaces;

public interface IDbContextSetup
{
    void OnModelCreating(ModelBuilder modelBuilder);
}