using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class CanCompleteToDoItemService
{
    public Task<bool> IsCanCompleteAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity, TimeSpan offset)
    {
        var result = IsCanComplete(entity, offset);

        if (!result)
        {
            return false.ToTaskResult();
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                return IsCompletedItemsAsync(context, entity, offset);
            case ToDoItemChildrenType.IgnoreCompletion:
                return true.ToTaskResult();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> IsCompletedItemsAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        foreach (var item in items)
        {
            var isCompleted = await IsCompletedAsync(context, item, offset);

            if (!isCompleted)
            {
                return false;
            }
        }

        return true;
    }

    private Task<bool> IsCompletedAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity, TimeSpan offset)
    {
        var now = DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly();

        return entity.Type switch
        {
            ToDoItemType.Value => entity.IsCompleted.ToTaskResult(),
            ToDoItemType.Step => entity.IsCompleted.ToTaskResult(),
            ToDoItemType.Group => IsCompletedItemsAsync(context, entity, offset),
            ToDoItemType.Planned => (entity.IsCompleted || entity.DueDate > now).ToTaskResult(),
            ToDoItemType.Periodicity => (entity.DueDate > now).ToTaskResult(),
            ToDoItemType.PeriodicityOffset => (entity.DueDate > now).ToTaskResult(),
            ToDoItemType.Circle => entity.IsCompleted.ToTaskResult(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool IsCanComplete(ToDoItemEntity entity, TimeSpan offset)
    {
        var now = DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly();

        return entity.Type switch
        {
            ToDoItemType.Value => !entity.IsCompleted,
            ToDoItemType.Step => !entity.IsCompleted,
            ToDoItemType.Group => false,
            ToDoItemType.Planned => !entity.IsCompleted && entity.DueDate <= now,
            ToDoItemType.Periodicity => entity.DueDate <= now,
            ToDoItemType.PeriodicityOffset => entity.DueDate <= now,
            ToDoItemType.Circle => !entity.IsCompleted,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}