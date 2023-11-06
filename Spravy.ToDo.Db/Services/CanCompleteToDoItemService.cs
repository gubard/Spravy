using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class CanCompleteToDoItemService
{
    public async Task<bool> IsCanCompleteAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>()
            .AsNoTracking()
            .Where(x => x.ParentId == entity.Id)
            .ToArrayAsync();

        var result = IsCanComplete(entity);

        if (!result)
        {
            return false;
        }

        switch (entity.ChildrenType)
        {
            case ToDoItemChildrenType.RequireCompletion:
                foreach (var item in items)
                {
                    result = IsCompleted(item);

                    if (!result)
                    {
                        return false;
                    }
                }

                break;
            case ToDoItemChildrenType.IgnoreCompletion:
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }
    
    private bool IsCompleted(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => entity.IsCompleted,
            ToDoItemType.Group => true,
            ToDoItemType.Planned => entity.IsCompleted,
            ToDoItemType.Periodicity => entity.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.PeriodicityOffset => entity.DueDate.ToCurrentDay() > DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.Circle => entity.IsCompleted,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool IsCanComplete(ToDoItemEntity entity)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => !entity.IsCompleted,
            ToDoItemType.Group => false,
            ToDoItemType.Planned => !entity.IsCompleted,
            ToDoItemType.Periodicity => entity.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.PeriodicityOffset => entity.DueDate.ToCurrentDay() <= DateTimeOffset.Now.ToCurrentDay(),
            ToDoItemType.Circle => !entity.IsCompleted,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}