using AutoMapper;
using Spravy.Db.Models;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Db.Core.Profiles;

public class SpravyDbProfile : Profile
{
    public const string StatusName = "Status";
    public const string ParentsName = "Parents";
    public const string ItemsName = "Items";
    public const string ActiveName = "Active";

    public SpravyDbProfile()
    {
        CreateMap<ToDoItemEntity, ActiveToDoItem>()
            .ConvertUsing((source, _, _) => new ActiveToDoItem(source.Id, source.Name));

        CreateMap<ToDoItemEntity, IToDoSubItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.Type switch
                {
                    ToDoItemType.Value => new ToDoSubItemValue(
                        source.Id,
                        source.Name,
                        source.IsCompleted,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.IsCurrent,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName]
                    ),
                    ToDoItemType.Group => new ToDoSubItemGroup(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsCurrent,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName]
                    ),
                    ToDoItemType.Planned => new ToDoSubItemPlanned(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsCurrent,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.DueDate,
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.IsCompleted
                    ),
                    ToDoItemType.Periodicity => new ToDoSubItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsCurrent,
                        source.DueDate,
                        source.TypeOfPeriodicity,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<ToDoItemEntity, IToDoItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.Type switch
                {
                    ToDoItemType.Value => new ToDoItemValue(
                        source.Id,
                        source.Name,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsCompleted,
                        source.Description,
                        source.IsCurrent
                    ),
                    ToDoItemType.Group => new ToDoItemGroup(
                        source.Id,
                        source.Name,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.Description,
                        source.IsCurrent
                    ),
                    ToDoItemType.Planned => new ToDoItemPlanned(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsCurrent,
                        source.DueDate,
                        source.IsCompleted
                    ),
                    ToDoItemType.Periodicity => new ToDoItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsCurrent,
                        source.DueDate,
                        source.TypeOfPeriodicity
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
    }
}