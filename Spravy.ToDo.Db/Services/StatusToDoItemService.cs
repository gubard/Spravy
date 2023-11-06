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
        return entity.Type switch
        {
            ToDoItemType.Value => GetValueStatusAsync(context, entity),
            ToDoItemType.Group => GetGroupStatusAsync(context, entity),
            ToDoItemType.Planned => GetPlannedStatusAsync(context, entity),
            ToDoItemType.Periodicity => GetDueDateStatusAsync(entity),
            ToDoItemType.PeriodicityOffset => GetDueDateStatusAsync(entity),
            ToDoItemType.Circle => GetCircleStatusAsync(context, entity),
            ToDoItemType.Step => GetStepStatusAsync(context, entity),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<ToDoItemStatus> GetPlannedStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        if (entity.DueDate.ToDayDateTimeWithOffset() > DateTimeOffset.Now.ToDayDateTimeWithOffset())
        {
            return ToDoItemStatus.Planned;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item);

            switch (status)
            {
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete: break;
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return await GetDueDateStatusAsync(entity);
    }

    private Task<ToDoItemStatus> GetDueDateStatusAsync(ToDoItemEntity entity)
    {
        if (entity.DueDate.ToDayDateTimeWithOffset() == DateTimeOffset.Now.ToDayDateTimeWithOffset())
        {
            return ToDoItemStatus.ReadyForComplete.ToTaskResult();
        }

        if (entity.DueDate.ToDayDateTimeWithOffset() < DateTimeOffset.Now.ToDayDateTimeWithOffset())
        {
            return ToDoItemStatus.Miss.ToTaskResult();
        }

        if (entity.DueDate.ToDayDateTimeWithOffset() > DateTimeOffset.Now.ToDayDateTimeWithOffset())
        {
            return ToDoItemStatus.Planned.ToTaskResult();
        }

        throw new ArgumentOutOfRangeException();
    }

    private async Task<ToDoItemStatus> GetGroupStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
    {
        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();
        var completedCount = 0;
        var isReadyForComplete = false;

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item);

            switch (status)
            {
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete:
                {
                    isReadyForComplete = true;

                    break;
                }
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed:
                {
                    completedCount++;

                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if (completedCount == items.Length)
        {
            return ToDoItemStatus.Completed;
        }

        if (isReadyForComplete)
        {
            return ToDoItemStatus.ReadyForComplete;
        }

        return ToDoItemStatus.Planned;
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
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete: break;
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return ToDoItemStatus.ReadyForComplete;
    }

    private async Task<ToDoItemStatus> GetCircleStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
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
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete: break;
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return ToDoItemStatus.ReadyForComplete;
    }
    
    private async Task<ToDoItemStatus> GetStepStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity)
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
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete: break;
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return ToDoItemStatus.ReadyForComplete;
    }
}