using AutoMapper;
using Spravy.ToDo.Db.Extensions;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Mapper.Profiles;

public class SpravyToDoDbProfile : Profile
{
    public const string StatusName = "Status";
    public const string ParentsName = "Parents";
    public const string ItemsName = "Items";
    public const string ActiveName = "Active";

    public SpravyToDoDbProfile()
    {
        CreateMap<ToDoItemEntity, DailyPeriodicity>();
        CreateMap<ToDoItemEntity, ToDoShortItem>();
        CreateMap<ToDoItemEntity, MonthlyPeriodicity>().ConstructUsing(x => new(x.GetDaysOfMonth()));
        CreateMap<ToDoItemEntity, WeeklyPeriodicity>().ConstructUsing(x => new(x.GetDaysOfWeek()));
        CreateMap<AddRootToDoItemOptions, ToDoItemEntity>();
        CreateMap<AddToDoItemOptions, ToDoItemEntity>();

        CreateMap<ToDoItemEntity, ToDoItemParent>()
            .ConstructUsing(x => new(x.Id, x.Name));

        CreateMap<ToDoItemEntity, AnnuallyPeriodicity>()
            .ConstructUsing(x => new(x.GetDaysOfYear()));

        CreateMap<ToDoItemEntity, ActiveToDoItem>()
            .ConvertUsing((source, _, _) => new(source.Id, source.Name));

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
                        source.IsPinned,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.LastCompleted
                    ),
                    ToDoItemType.Group => new ToDoSubItemGroup(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsPinned,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName]
                    ),
                    ToDoItemType.Planned => new ToDoSubItemPlanned(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsPinned,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.DueDate,
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.IsCompleted,
                        source.LastCompleted
                    ),
                    ToDoItemType.Periodicity => new ToDoSubItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsPinned,
                        source.DueDate,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.LastCompleted
                    ),
                    ToDoItemType.PeriodicityOffset => new ToDoSubItemPeriodicityOffset(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)resolutionContext.Items[StatusName],
                        source.Description,
                        source.IsPinned,
                        source.DueDate,
                        (ActiveToDoItem?)resolutionContext.Items[ActiveName],
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.LastCompleted
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
                        source.IsPinned,
                        source.ChildrenType
                    ),
                    ToDoItemType.Group => new ToDoItemGroup(
                        source.Id,
                        source.Name,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.Description,
                        source.IsPinned
                    ),
                    ToDoItemType.Planned => new ToDoItemPlanned(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsPinned,
                        source.DueDate,
                        source.IsCompleted,
                        source.ChildrenType
                    ),
                    ToDoItemType.Periodicity => new ToDoItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsPinned,
                        source.DueDate,
                        source.TypeOfPeriodicity switch
                        {
                            TypeOfPeriodicity.Daily => resolutionContext.Mapper.Map<DailyPeriodicity>(source),
                            TypeOfPeriodicity.Weekly => resolutionContext.Mapper.Map<WeeklyPeriodicity>(source),
                            TypeOfPeriodicity.Monthly => resolutionContext.Mapper.Map<MonthlyPeriodicity>(source),
                            TypeOfPeriodicity.Annually => resolutionContext.Mapper.Map<AnnuallyPeriodicity>(source),
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        source.ChildrenType
                    ),
                    ToDoItemType.PeriodicityOffset => new ToDoItemPeriodicityOffset(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])resolutionContext.Items[ItemsName],
                        (ToDoItemParent[])resolutionContext.Items[ParentsName],
                        source.IsPinned,
                        source.DaysOffset,
                        source.MonthsOffset,
                        source.WeeksOffset,
                        source.YearsOffset,
                        source.DueDate,
                        source.ChildrenType
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
    }
}