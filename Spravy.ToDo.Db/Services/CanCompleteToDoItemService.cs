using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class CanCompleteToDoItemService
{
    public Task<bool> IsCanCompleteAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var result = IsCanComplete(entity);

        if (!result)
        {
            return false.ToTaskResult();
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                return IsCompletedItemsAsync(context, entity);
            case ToDoItemChildrenType.IgnoreCompletion:
                return true.ToTaskResult();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> IsCompletedItemsAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        foreach (var item in items)
        {
            if (!await IsCompletedAsync(context, item))
            {
                return false;
            }
        }

        return true;
    }

    private Task<bool> IsCompletedAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => entity.IsCompleted.ToTaskResult(),
            ToDoItemType.Step => entity.IsCompleted.ToTaskResult(),
            ToDoItemType.Group => IsCompletedItemsAsync(context, entity),
            ToDoItemType.Planned => entity.IsCompleted.ToTaskResult(),
            ToDoItemType.Periodicity => (entity.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
                .ToTaskResult(),
            ToDoItemType.PeriodicityOffset => (entity.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay())
                .ToTaskResult(),
            ToDoItemType.Circle => entity.IsCompleted.ToTaskResult(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool IsCanComplete(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => !entity.IsCompleted,
            ToDoItemType.Step => !entity.IsCompleted,
            ToDoItemType.Group => false,
            ToDoItemType.Planned => !entity.IsCompleted,
            ToDoItemType.Periodicity => entity.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.PeriodicityOffset => entity.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.Circle => !entity.IsCompleted,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}