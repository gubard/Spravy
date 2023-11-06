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
                (source, _, context) => source.Type switch
                {
                    ToDoItemType.Value => new ToDoSubItemValue(
                        source.Id,
                        source.Name,
                        source.IsCompleted,
                        source.OrderIndex,
                        (ToDoItemStatus)context.Items[StatusName],
                        source.Description,
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.IsFavorite,
                        (ActiveToDoItem?)context.Items[ActiveName],
                        source.LastCompleted,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Group => new ToDoSubItemGroup(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)context.Items[StatusName],
                        source.Description,
                        source.IsFavorite,
                        (ActiveToDoItem?)context.Items[ActiveName],
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Planned => new ToDoSubItemPlanned(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)context.Items[StatusName],
                        source.Description,
                        source.IsFavorite,
                        (ActiveToDoItem?)context.Items[ActiveName],
                        source.DueDate,
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.IsCompleted,
                        source.LastCompleted,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Periodicity => new ToDoSubItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)context.Items[StatusName],
                        source.Description,
                        source.IsFavorite,
                        source.DueDate,
                        (ActiveToDoItem?)context.Items[ActiveName],
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.LastCompleted,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.PeriodicityOffset => new ToDoSubItemPeriodicityOffset(
                        source.Id,
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)context.Items[StatusName],
                        source.Description,
                        source.IsFavorite,
                        source.DueDate,
                        (ActiveToDoItem?)context.Items[ActiveName],
                        source.CompletedCount,
                        source.SkippedCount,
                        source.FailedCount,
                        source.LastCompleted,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<ToDoItemEntity, IToDoItem>()
            .ConvertUsing(
                (source, _, context) => source.Type switch
                {
                    ToDoItemType.Value => new ToDoItemValue(
                        source.Id,
                        source.Name,
                        (IToDoSubItem[])context.Items[ItemsName],
                        (ToDoItemParent[])context.Items[ParentsName],
                        source.IsCompleted,
                        source.Description,
                        source.IsFavorite,
                        source.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Group => new ToDoItemGroup(
                        source.Id,
                        source.Name,
                        (IToDoSubItem[])context.Items[ItemsName],
                        (ToDoItemParent[])context.Items[ParentsName],
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Planned => new ToDoItemPlanned(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])context.Items[ItemsName],
                        (ToDoItemParent[])context.Items[ParentsName],
                        source.IsFavorite,
                        source.DueDate,
                        source.IsCompleted,
                        source.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.Periodicity => new ToDoItemPeriodicity(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])context.Items[ItemsName],
                        (ToDoItemParent[])context.Items[ParentsName],
                        source.IsFavorite,
                        source.DueDate,
                        source.TypeOfPeriodicity switch
                        {
                            TypeOfPeriodicity.Daily => context.Mapper.Map<DailyPeriodicity>(source),
                            TypeOfPeriodicity.Weekly => context.Mapper.Map<WeeklyPeriodicity>(source),
                            TypeOfPeriodicity.Monthly => context.Mapper.Map<MonthlyPeriodicity>(source),
                            TypeOfPeriodicity.Annually => context.Mapper.Map<AnnuallyPeriodicity>(source),
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        source.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    ToDoItemType.PeriodicityOffset => new ToDoItemPeriodicityOffset(
                        source.Id,
                        source.Name,
                        source.Description,
                        (IToDoSubItem[])context.Items[ItemsName],
                        (ToDoItemParent[])context.Items[ParentsName],
                        source.IsFavorite,
                        source.DaysOffset,
                        source.MonthsOffset,
                        source.WeeksOffset,
                        source.YearsOffset,
                        source.DueDate,
                        source.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link ?? string.Empty)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
    }
}