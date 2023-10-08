using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class StatusToDoItemService
{
    public Task<ToDoItemStatus> GetStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
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
                return GetPlannedStatusAsync(context, entity);
            }
            case ToDoItemType.Periodicity:
            {
                return GetDueDateStatusAsync(entity);
            }
            case ToDoItemType.PeriodicityOffset:
            {
                return GetDueDateStatusAsync(entity);
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task<ToDoItemStatus> GetPlannedStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        if (entity.DueDate > DateTimeOffset.Now.ToCurrentDay())
        {
            return ToDoItemStatus.Planned;
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
                case ToDoItemStatus.Planned:
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

        return await GetDueDateStatusAsync(entity);
    }

    private Task<ToDoItemStatus> GetDueDateStatusAsync(ToDoItemEntity entity)
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
            return ToDoItemStatus.Planned.ToTaskResult();
        }

        throw new ArgumentOutOfRangeException();
    }

    private async Task<ToDoItemStatus> GetGroupStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();
        var completedCount = 0;

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
                case ToDoItemStatus.Planned:
                {
                    break;
                }
                case ToDoItemStatus.Completed:
                {
                    completedCount++;

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        if (completedCount == items.Length)
        {
            return ToDoItemStatus.Completed;
        }

        return ToDoItemStatus.ReadyForComplete;
    }

    private async Task<ToDoItemStatus> GetValueStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
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
                case ToDoItemStatus.Planned:
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