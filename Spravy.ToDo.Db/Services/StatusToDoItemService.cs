using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class StatusToDoItemService
{
    public Task<ToDoItemStatus> GetStatusAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        switch (entity.Type)
        {
            case ToDoItemType.Value:
            {
                return GetValueStatusAsync(context, entity);
            }
            case ToDoItemType.Group:
            {
                return GetGroupStatusAsync(context, entity);
            }
            case ToDoItemType.Planned:
            {
                return GetDueDateStatusAsync(context, entity);
            }
            case ToDoItemType.Periodicity:
            {
                return GetDueDateStatusAsync(context, entity);
            }
            case ToDoItemType.PeriodicityOffset:
            {
                return GetDueDateStatusAsync(context, entity);
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    private Task<ToDoItemStatus> GetDueDateStatusAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.DueDate == DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.ReadyForComplete.ToTaskResult();
        }

        if (entity.DueDate < DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.Miss.ToTaskResult();
        }

        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.Completed.ToTaskResult();
        }

        throw new ArgumentOutOfRangeException();
    }

    private async Task<ToDoItemStatus> GetGroupStatusAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item);

            switch (status)
            {
                case ToDoItemStatus.Miss:
                {
                    return ToDoItemStatus.Miss;
                }
                case ToDoItemStatus.ReadyForComplete:
                {
                    return ToDoItemStatus.ReadyForComplete;
                }
                case ToDoItemStatus.Completed:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        return ToDoItemStatus.Completed;
    }

    private async Task<ToDoItemStatus> GetValueStatusAsync(SpravyToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item);

            switch (status)
            {
                case ToDoItemStatus.Miss:
                {
                    return ToDoItemStatus.Miss;
                }
                case ToDoItemStatus.ReadyForComplete:
                {
                    break;
                }
                case ToDoItemStatus.Completed:
                {
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        return ToDoItemStatus.ReadyForComplete;
    }
}