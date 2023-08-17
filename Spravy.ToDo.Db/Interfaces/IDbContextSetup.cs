using Microsoft.EntityFrameworkCore;

namespace Spravy.ToDo.Db.Interfaces;

public interface IDbContextSetup
{
    void OnModelCreating(ModelBuilder modelBuilder);
}