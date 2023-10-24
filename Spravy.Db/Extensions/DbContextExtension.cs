using Microsoft.EntityFrameworkCore;

namespace Spravy.Db.Extensions;

public static class DbContextExtension
{
    public static async Task ExecuteSaveChangesAsync<TDbContext>(this TDbContext context, Func<TDbContext, Task> func)
        where TDbContext : DbContext
    {
        await func.Invoke(context);
        await context.SaveChangesAsync();
    }

    public static async ValueTask ExecuteSaveChangesAsync<TDbContext>(
        this TDbContext context,
        Func<TDbContext, ValueTask> func
    )
        where TDbContext : DbContext
    {
        await func.Invoke(context);
        await context.SaveChangesAsync();
    }

    public static async ValueTask<TResult> ExecuteSaveChangesAsync<TDbContext, TResult>(
        this TDbContext context,
        Func<TDbContext, ValueTask<TResult>> func
    )
        where TDbContext : DbContext
    {
        var result = await func.Invoke(context);
        await context.SaveChangesAsync();

        return result;
    }

    public static async Task ExecuteSaveChangesTransactionAsync<TDbContext>(
        this TDbContext context,
        Func<TDbContext, Task> func
    )
        where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await context.ExecuteSaveChangesAsync(func);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public static async ValueTask ExecuteSaveChangesTransactionValueAsync<TDbContext>(
        this TDbContext context,
        Func<TDbContext, ValueTask> func
    )
        where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await context.ExecuteSaveChangesAsync(func);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public static async ValueTask<TResult> ExecuteSaveChangesTransactionAsync<TDbContext, TResult>(
        this TDbContext context,
        Func<TDbContext, ValueTask<TResult>> func
    )
        where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var result = await context.ExecuteSaveChangesAsync(func);
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}