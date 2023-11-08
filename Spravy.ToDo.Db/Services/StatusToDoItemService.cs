using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Services;

public class StatusToDoItemService
{
    public Task<ToDoItemStatus> GetStatusAsync(SpravyDbToDoDbContext context, ToDoItemEntity entity, TimeSpan offset)
    {
        return entity.Type switch
        {
            ToDoItemType.Value => GetValueStatusAsync(context, entity, offset),
            ToDoItemType.Group => GetGroupStatusAsync(context, entity, offset),
            ToDoItemType.Planned => GetPlannedStatusAsync(context, entity, offset),
            ToDoItemType.Periodicity => GetDueDateStatusAsync(entity, offset),
            ToDoItemType.PeriodicityOffset => GetDueDateStatusAsync(entity, offset),
            ToDoItemType.Circle => GetCircleStatusAsync(context, entity, offset),
            ToDoItemType.Step => GetStepStatusAsync(context, entity, offset),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<ToDoItemStatus> GetPlannedStatusAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToDoItemStatus.Planned;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item, offset);

            switch (status)
            {
                case ToDoItemStatus.Miss: return ToDoItemStatus.Miss;
                case ToDoItemStatus.ReadyForComplete: break;
                case ToDoItemStatus.Planned: break;
                case ToDoItemStatus.Completed: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return await GetDueDateStatusAsync(entity, offset);
    }

    private Task<ToDoItemStatus> GetDueDateStatusAsync(ToDoItemEntity entity, TimeSpan offset)
    {
        if (entity.DueDate == DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToDoItemStatus.ReadyForComplete.ToTaskResult();
        }

        if (entity.DueDate < DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToDoItemStatus.Miss.ToTaskResult();
        }

        if (entity.DueDate > DateTimeOffset.UtcNow.Add(offset).Date.ToDateOnly())
        {
            return ToDoItemStatus.Planned.ToTaskResult();
        }

        throw new ArgumentOutOfRangeException();
    }

    private async Task<ToDoItemStatus> GetGroupStatusAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();
        var completedCount = 0;
        var isReadyForComplete = false;

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item, offset);

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

    private async Task<ToDoItemStatus> GetValueStatusAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item, offset);

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

    private async Task<ToDoItemStatus> GetCircleStatusAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item, offset);

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

    private async Task<ToDoItemStatus> GetStepStatusAsync(
        SpravyDbToDoDbContext context,
        ToDoItemEntity entity,
        TimeSpan offset
    )
    {
        if (entity.IsCompleted)
        {
            return ToDoItemStatus.Completed;
        }

        var items = await context.Set<ToDoItemEntity>().Where(x => x.ParentId == entity.Id).ToArrayAsync();

        foreach (var item in items)
        {
            var status = await GetStatusAsync(context, item, offset);

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