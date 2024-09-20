using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Db.Extensions;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ToDoDbMapper
{
    public static partial PlannedToDoItemSettings ToPlannedToDoItemSettings(
        this ToDoItemEntity entity
    );

    public static partial ValueToDoItemSettings ToValueToDoItemSettings(this ToDoItemEntity entity);

    public static partial PeriodicityToDoItemSettings ToPeriodicityToDoItemSettings(
        this ToDoItemEntity entity
    );

    public static partial ToDoShortItem ToToDoShortItem(this ToDoItemEntity entity);

    public static partial ReadOnlyMemory<ToDoShortItem> ToToDoShortItem(
        this ReadOnlyMemory<ToDoItemEntity> entity
    );

    public static partial ToDoItemEntity ToToDoItemEntity(this AddToDoItemOptions entity);

    public static partial ActiveToDoItem ToActiveToDoItem(this ToDoItemEntity entity);

    public static AnnuallyPeriodicity ToAnnuallyPeriodicity(this ToDoItemEntity entity)
    {
        return new(entity.GetDaysOfYear());
    }

    public static WeeklyPeriodicity ToWeeklyPeriodicity(this ToDoItemEntity entity)
    {
        return new(entity.GetDaysOfWeek());
    }

    public static MonthlyPeriodicity ToMonthlyPeriodicity(this ToDoItemEntity entity)
    {
        return new(entity.GetDaysOfMonth());
    }

    public static DailyPeriodicity ToDailyPeriodicity(this ToDoItemEntity entity)
    {
        return new();
    }

    public static partial PeriodicityOffsetToDoItemSettings ToPeriodicityOffsetToDoItemSettings(
        this ToDoItemEntity entity
    );

    public static ToDoItem ToToDoItem(this ToDoItemEntity entity, ToDoItemParameters parameters)
    {
        return new(
            entity.Id,
            entity.Name,
            entity.IsFavorite,
            entity.Type,
            entity.Description,
            entity.Link.ToOptionUri(),
            entity.OrderIndex,
            parameters.Status,
            parameters.ActiveItem,
            parameters.IsCan,
            entity.ParentId.ToOption(),
            entity.DescriptionType,
            GetReferenceId(entity),
            entity.IsBookmark
        );
    }

    private static OptionStruct<Guid> GetReferenceId(ToDoItemEntity item)
    {
        if (item.Type != ToDoItemType.Reference)
        {
            return OptionStruct<Guid>.Default;
        }

        if (item.ReferenceId == item.Id)
        {
            return OptionStruct<Guid>.Default;
        }

        return item.ReferenceId.ToOption();
    }

    private static string MapToString(Option<Uri> value)
    {
        return value.MapToString();
    }

    private static Guid? ToNullableGuid(OptionStruct<Guid> value)
    {
        return value.ToNullableGuid();
    }

    private static OptionStruct<Guid> ToOptionGuid(Guid? value)
    {
        return value.ToOptionGuid();
    }

    public static FullToDoItem ToFullToDoItem(
        this ToDoItemEntity entity,
        ToDoItemParameters parameters
    )
    {
        var referenceId =
            entity.Type == ToDoItemType.Reference
                ? entity.ReferenceId.ToOptionGuid()
                : OptionStruct<Guid>.Default;

        return new(
            entity.Id,
            entity.Name,
            entity.IsFavorite,
            entity.Type,
            entity.Description,
            entity.Link.ToOptionUri(),
            entity.OrderIndex,
            parameters.Status,
            parameters.ActiveItem,
            parameters.IsCan,
            entity.ParentId.ToOption(),
            entity.DescriptionType,
            referenceId,
            entity.GetDaysOfYear(),
            entity.GetDaysOfMonth(),
            entity.ChildrenType,
            entity.DueDate,
            entity.DaysOffset,
            entity.MonthsOffset,
            entity.WeeksOffset,
            entity.YearsOffset,
            entity.IsRequiredCompleteInDueDate,
            entity.TypeOfPeriodicity,
            entity.GetDaysOfWeek(),
            entity.IsBookmark
        );
    }
}
