using Google.Protobuf.WellKnownTypes;

namespace Spravy.ToDo.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ToDoMapper
{
    public static partial ResetToDoItemOptions ToResetToDoItemOptions(
        this ResetToDoItemOptionsGrpc value
    );

    public static partial ResetToDoItemOptionsGrpc ToResetToDoItemOptionsGrpc(
        this ResetToDoItemOptions value
    );

    public static partial DayOfWeek ToDayOfWeek(this DayOfWeekGrpc value);

    public static partial DayOfWeekGrpc ToDayOfWeek(this DayOfWeek value);

    public static partial TypeOfPeriodicity ToTypeOfPeriodicity(this TypeOfPeriodicityGrpc value);

    public static partial DescriptionType ToDescriptionType(this DescriptionTypeGrpc value);

    public static partial ToDoItemChildrenType ToChildrenType(this ToDoItemChildrenTypeGrpc value);

    public static partial DescriptionTypeGrpc ToDescriptionTypeGrpc(this DescriptionType value);

    public static partial FullToDoItem ToFullToDoItem(this FullToDoItemGrpc value);

    public static partial ReadOnlyMemory<FullToDoItem> ToFullToDoItem(
        this IEnumerable<FullToDoItemGrpc> value
    );

    public static partial ReadOnlyMemory<FullToDoItemGrpc> ToFullToDoItemGrpc(
        this ReadOnlyMemory<FullToDoItem> value
    );

    public static partial FullToDoItemGrpc ToFullToDoItemGrpc(this FullToDoItem value);

    public static partial ToDoShortItemGrpc ToToDoShortItemGrpc(this ToDoShortItem value);

    public static partial ToDoShortItem ToToDoShortItem(this ToDoShortItemGrpc value);

    public static partial ReadOnlyMemory<ToDoShortItemGrpc> ToToDoShortItemGrpc(
        this ReadOnlyMemory<ToDoShortItem> value
    );

    public static partial ReadOnlyMemory<ToDoShortItem> ToToDoShortItem(
        this IEnumerable<ToDoShortItemGrpc> value
    );

    public static partial AddToDoItemOptionsGrpc ToAddToDoItemOptionsGrpc(
        this AddToDoItemOptions value
    );

    public static partial AddToDoItemOptions ToAddToDoItemOptions(
        this AddToDoItemOptionsGrpc value
    );

    public static partial DayOfYearGrpc ToDayOfYearGrpc(this DayOfYear value);

    public static partial DayOfYear ToDayOfYear(this DayOfYearGrpc value);

    public static partial ToDoItemTypeGrpc ToToDoItemTypeGrpc(this ToDoItemType value);

    public static partial ToDoItemType ToToDoItemType(this ToDoItemTypeGrpc value);

    public static partial ToDoItemIsCanGrpc ToToDoItemIsCanGrpc(this ToDoItemIsCan value);

    public static partial ToDoItemIsCan ToToDoItemIsCan(this ToDoItemIsCanGrpc value);

    public static partial ToDoItemToStringOptionsGrpc ToToDoItemToStringOptionsGrpc(
        this ToDoItemToStringOptions value
    );

    public static partial ToDoItemToStringOptions ToToDoItemToStringOptions(
        this ToDoItemToStringOptionsGrpc value
    );

    public static partial ToDoSelectorItemGrpc ToToDoSelectorItemGrpc(this ToDoSelectorItem value);

    public static partial ReadOnlyMemory<ToDoSelectorItemGrpc> ToToDoSelectorItemGrpc(
        this ReadOnlyMemory<ToDoSelectorItem> value
    );

    public static partial ToDoSelectorItem ToToDoSelectorItem(this ToDoSelectorItemGrpc value);

    public static partial ReadOnlyMemory<ToDoSelectorItem> ToToDoSelectorItem(
        this IEnumerable<ToDoSelectorItemGrpc> value
    );

    public static ToDoShortItemGrpc? ToToDoShortItemNullableGrpc(
        this OptionStruct<ToDoShortItem> value
    )
    {
        if (value.TryGetValue(out var item))
        {
            return item.ToToDoShortItemGrpc();
        }

        return null;
    }

    public static OptionStruct<ToDoShortItem> ToOptionToDoShortItem(this ToDoShortItemGrpc? value)
    {
        if (value is null)
        {
            return new();
        }

        return new(value.ToToDoShortItem());
    }

    public static partial UpdateOrderIndexToDoItemOptionsGrpc ToUpdateOrderIndexToDoItemOptionsGrpc(
        this UpdateOrderIndexToDoItemOptions value
    );

    public static partial UpdateOrderIndexToDoItemOptions ToUpdateOrderIndexToDoItemOptions(
        this UpdateOrderIndexToDoItemOptionsGrpc value
    );

    public static EditToDoItems ToEditToDoItems(this EditToDoItemsGrpc value)
    {
        return new(
            value.Ids.ToGuid(),
            value.Name.IsEdit ? new(value.Name.Value) : new(),
            value.IsFavorite.IsEdit ? new(value.IsFavorite.Value) : new(),
            value.Type.IsEdit ? new(value.Type.Value.ToToDoItemType()) : new(),
            value.Description.IsEdit ? new(value.Description.Value) : new(),
            value.Link.IsEdit ? new(value.Link.Value.ToOptionUri()) : new(),
            value.ParentId.IsEdit ? new(value.ParentId.Value.ToOptionGuid()) : new(),
            value.DescriptionType.IsEdit
                ? new(value.DescriptionType.Value.ToDescriptionType())
                : new(),
            value.ReferenceId.IsEdit ? new(value.ReferenceId.Value.ToOptionGuid()) : new(),
            value.AnnuallyDays.IsEdit
                ? new(value.AnnuallyDays.Value.Select(x => x.ToDayOfYear()).ToArray())
                : new(),
            value.MonthlyDays.IsEdit
                ? new(value.MonthlyDays.Value.Select(x => (byte)x).ToArray())
                : new(),
            value.ChildrenType.IsEdit ? new(value.ChildrenType.Value.ToChildrenType()) : new(),
            value.DueDate.IsEdit ? new(value.DueDate.Value.ToDateOnly()) : new(),
            value.DaysOffset.IsEdit ? new((ushort)value.DaysOffset.Value) : new(),
            value.MonthsOffset.IsEdit ? new((ushort)value.MonthsOffset.Value) : new(),
            value.WeeksOffset.IsEdit ? new((ushort)value.WeeksOffset.Value) : new(),
            value.YearsOffset.IsEdit ? new((ushort)value.YearsOffset.Value) : new(),
            value.IsRequiredCompleteInDueDate.IsEdit
                ? new(value.IsRequiredCompleteInDueDate.Value)
                : new(),
            value.TypeOfPeriodicity.IsEdit
                ? new(value.TypeOfPeriodicity.Value.ToTypeOfPeriodicity())
                : new(),
            value.WeeklyDays.IsEdit
                ? new(value.WeeklyDays.Value.Select(x => x.ToDayOfWeek()).ToArray())
                : new(),
            value.IsBookmark.IsEdit ? new(value.IsBookmark.Value) : new(),
            value.Icon.IsEdit ? new(value.Icon.Value) : new(),
            value.Color.IsEdit ? new(value.Color.Value) : new()
        );
    }

    public static EditToDoItemsGrpc ToEditToDoItemsGrpc(this EditToDoItems value)
    {
        var edit = new EditToDoItemsGrpc
        {
            Description = value.Description.ToEditPropertyStringGrpc(),
            Icon = value.Icon.ToEditPropertyStringGrpc(),
            Link = value.Link.ToEditPropertyStringGrpc(),
            Name = value.Name.ToEditPropertyStringGrpc(),
            Type = value.Type.ToEditPropertyToDoItemTypeGrpc(),
            AnnuallyDays = value.AnnuallyDays.ToEditPropertyDaysOfYearGrpc(),
            ParentId = value.ParentId.ToEditPropertyBytesGrpc(),
            IsBookmark = value.IsBookmark.ToEditPropertyBooleanGrpc(),
            IsFavorite = value.IsFavorite.ToEditPropertyBooleanGrpc(),
            MonthlyDays = value.MonthlyDays.ToEditPropertyUInt32sGrpc(),
            ChildrenType = value.ChildrenType.ToEditPropertyToDoItemChildrenTypeGrpc(),
            DaysOffset = value.DaysOffset.ToEditPropertyUInt32Grpc(),
            DescriptionType = value.DescriptionType.ToEditPropertyDescriptionTypeGrpc(),
            DueDate = value.DueDate.ToEditPropertyTimestampGrpc(),
            MonthsOffset = value.MonthsOffset.ToEditPropertyUInt32Grpc(),
            YearsOffset = value.YearsOffset.ToEditPropertyUInt32Grpc(),
            ReferenceId = value.ReferenceId.ToEditPropertyBytesGrpc(),
            WeeklyDays = value.WeeklyDays.ToEditPropertyDaysOfWeekGrpc(),
            WeeksOffset = value.WeeksOffset.ToEditPropertyUInt32Grpc(),
            TypeOfPeriodicity = value.TypeOfPeriodicity.ToEditPropertyTypeOfPeriodicityGrpc(),
            IsRequiredCompleteInDueDate =
                value.IsRequiredCompleteInDueDate.ToEditPropertyBooleanGrpc(),
            Color = value.Color.ToEditPropertyStringGrpc(),
        };

        edit.Ids.AddRange(value.Ids.ToByteString().ToArray());

        return edit;
    }

    public static partial EditPropertyTypeOfPeriodicityGrpc ToEditPropertyTypeOfPeriodicityGrpc(
        this EditPropertyValue<TypeOfPeriodicity> value
    );

    public static partial EditPropertyDaysOfWeekGrpc ToEditPropertyDaysOfWeekGrpc(
        this EditPropertyValue<ReadOnlyMemory<DayOfWeek>> value
    );

    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(
        this EditPropertyValue<uint> value
    );

    public static EditPropertyTimestampGrpc ToEditPropertyTimestampGrpc(
        this EditPropertyValue<DateOnly> value
    )
    {
        if (value.IsEdit)
        {
            return new() { IsEdit = true, Value = value.Value.ToTimestamp() };
        }

        return new() { IsEdit = false, Value = null };
    }

    public static partial EditPropertyDescriptionTypeGrpc ToEditPropertyDescriptionTypeGrpc(
        this EditPropertyValue<DescriptionType> value
    );

    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(
        this EditPropertyValue<ushort> value
    );

    public static partial EditPropertyToDoItemChildrenTypeGrpc ToEditPropertyToDoItemChildrenTypeGrpc(
        this EditPropertyValue<ToDoItemChildrenType> value
    );

    public static partial EditPropertyUInt32sGrpc ToEditPropertyUInt32sGrpc(
        this EditPropertyValue<ReadOnlyMemory<byte>> value
    );

    public static partial EditPropertyBooleanGrpc ToEditPropertyBooleanGrpc(
        this EditPropertyValue<bool> value
    );

    public static EditPropertyBytesGrpc ToEditPropertyBytesGrpc(
        this EditPropertyValue<OptionStruct<Guid>> value
    )
    {
        if (!value.IsEdit)
        {
            return new() { IsEdit = false, Value = ByteString.Empty, };
        }

        return new() { IsEdit = true, Value = value.Value.ToByteString(), };
    }

    public static partial EditPropertyDaysOfYearGrpc ToEditPropertyDaysOfYearGrpc(
        this EditPropertyValue<ReadOnlyMemory<DayOfYear>> value
    );

    public static partial EditPropertyToDoItemTypeGrpc ToEditPropertyToDoItemTypeGrpc(
        this EditPropertyValue<ToDoItemType> value
    );

    public static EditPropertyStringGrpc ToEditPropertyStringGrpc(
        this EditPropertyValue<Option<Uri>> value
    )
    {
        if (!value.IsEdit)
        {
            return new() { IsEdit = false, Value = string.Empty, };
        }

        if (value.Value.TryGetValue(out var link))
        {
            return new() { IsEdit = false, Value = link.AbsoluteUri, };
        }

        return new() { IsEdit = true, Value = string.Empty, };
    }

    public static EditPropertyStringGrpc ToEditPropertyStringGrpc(
        this EditPropertyValue<string> value
    )
    {
        if (value.IsEdit)
        {
            return new() { IsEdit = true, Value = value.Value, };
        }

        return new() { IsEdit = false, Value = string.Empty, };
    }

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }

    private static ByteString ToByteString(ReadOnlyMemory<byte> bytes)
    {
        return bytes.ToByteString();
    }

    private static Guid ToGuid(ByteString byteString)
    {
        return byteString.ToGuid();
    }

    private static ByteString ToByteString(OptionStruct<Guid> id)
    {
        if (id.TryGetValue(out var value))
        {
            return value.ToByteString();
        }

        return ByteString.Empty;
    }

    private static OptionStruct<Guid> ToOptionGuid(ByteString byteString)
    {
        if (byteString.IsEmpty)
        {
            return new();
        }

        return new(byteString.ToGuid());
    }

    private static Option<Uri> ToOptionUri(string? str)
    {
        return str.ToOptionUri();
    }

    private static string MapToString(Option<Uri> str)
    {
        return str.MapToString();
    }

    private static Timestamp ToTimestamp(DateOnly value)
    {
        return value.ToTimestamp();
    }

    private static DateOnly ToDateOnly(Timestamp value)
    {
        return value.ToDateOnly();
    }
}
