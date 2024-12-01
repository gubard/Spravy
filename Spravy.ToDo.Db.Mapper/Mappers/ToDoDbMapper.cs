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
    public static ToDoShortItem ToToDoShortItem(this ToDoItemEntity entity)
    {
        return new(
            entity.Id,
            entity.Name,
            entity.OrderIndex,
            entity.Description,
            entity.Type,
            entity.IsBookmark,
            entity.IsFavorite,
            entity.DueDate,
            entity.TypeOfPeriodicity,
            entity.GetDaysOfYear(),
            entity.GetDaysOfMonth(),
            entity.GetDaysOfWeek(),
            entity.DaysOffset,
            entity.MonthsOffset,
            entity.WeeksOffset,
            entity.YearsOffset,
            entity.ChildrenType,
            entity.IsRequiredCompleteInDueDate,
            entity.DescriptionType,
            entity.Icon,
            entity.Color,
            GetReferenceId(entity),
            entity.ParentId.ToOptionGuid(),
            entity.Link.ToOptionUri(),
            entity.RemindDaysBefore
        );
    }

    public static partial ReadOnlyMemory<ToDoShortItem> ToToDoShortItem(this ReadOnlyMemory<ToDoItemEntity> entity);

    public static ToDoItemEntity ToToDoItemEntity(this AddToDoItemOptions value)
    {
        var result = new ToDoItemEntity
        {
            Color = value.Color,
            Description = value.Description,
            Icon = value.Icon,
            DescriptionType = value.DescriptionType,
            ChildrenType = value.ChildrenType,
            Type = value.Type,
            Link = value.Link.TryGetValue(out var link) ? link.AbsoluteUri : string.Empty,
            Name = value.Name,
            IsBookmark = value.IsBookmark,
            ReferenceId = value.ReferenceId.ToNullableGuid(),
            IsFavorite = value.IsFavorite,
            DaysOffset = value.DaysOffset,
            DueDate = value.DueDate,
            MonthsOffset = value.MonthsOffset,
            ParentId = value.ParentId.ToNullableGuid(),
            WeeksOffset = value.WeeksOffset,
            YearsOffset = value.YearsOffset,
            RemindDaysBefore = value.RemindDaysBefore,
            TypeOfPeriodicity = value.TypeOfPeriodicity,
            IsRequiredCompleteInDueDate = value.IsRequiredCompleteInDueDate,
            NormalizeName = value.Name.ToUpperInvariant(),
        };

        result.SetMonthlyDays(value.MonthlyDays);
        result.SetWeeklyDays(value.WeeklyDays);
        result.SetAnnuallyDays(value.AnnuallyDays);

        return result;
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

    public static FullToDoItem ToFullToDoItem(this ToDoItemEntity entity, ToDoItemParameters parameters)
    {
        return new(entity.ToToDoShortItem(), parameters.Status, parameters.ActiveItem, parameters.IsCan);
    }
}