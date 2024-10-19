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
            entity.Link.ToOptionUri()
        );
    }

    public static partial ReadOnlyMemory<ToDoShortItem> ToToDoShortItem(
        this ReadOnlyMemory<ToDoItemEntity> entity
    );

    public static partial ToDoItemEntity ToToDoItemEntity(this AddToDoItemOptions entity);

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
        return new(
            entity.ToToDoShortItem(),
            parameters.Status,
            parameters.ActiveItem,
            parameters.IsCan
        );
    }
}
