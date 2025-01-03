using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Spravy.Domain.Extensions;

namespace Spravy.ToDo.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ToDoMapper
{
    public static partial ResetToDoItemOptions ToResetToDoItemOptions(this ResetToDoItemOptionsGrpc value);
    public static partial ResetToDoItemOptionsGrpc ToResetToDoItemOptionsGrpc(this ResetToDoItemOptions value);
    public static partial GetToStringItemGrpc ToGetToStringItemGrpc(this GetToStringItem value);
    public static partial ReadOnlyMemory<FullToDoItem> ToFullToDoItem(this IEnumerable<FullToDoItemGrpc> value);
    public static partial EditPropertyDaysOfYearGrpc ToEditPropertyDaysOfYearGrpc(this EditPropertyValue<ReadOnlyMemory<DayOfYear>> value);
    public static partial EditPropertyToDoItemTypeGrpc ToEditPropertyToDoItemTypeGrpc(this EditPropertyValue<ToDoItemType> value);
    public static partial EditPropertyDescriptionTypeGrpc ToEditPropertyDescriptionTypeGrpc(this EditPropertyValue<DescriptionType> value);
    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(this EditPropertyValue<ushort> value);
    public static partial EditPropertyToDoItemChildrenTypeGrpc ToEditPropertyToDoItemChildrenTypeGrpc(this EditPropertyValue<ToDoItemChildrenType> value);
    public static partial EditPropertyUInt32sGrpc ToEditPropertyUInt32sGrpc(this EditPropertyValue<ReadOnlyMemory<byte>> value);
    public static partial EditPropertyBooleanGrpc ToEditPropertyBooleanGrpc(this EditPropertyValue<bool> value);
    public static partial EditPropertyTypeOfPeriodicityGrpc ToEditPropertyTypeOfPeriodicityGrpc(this EditPropertyValue<TypeOfPeriodicity> value);
    public static partial EditPropertyDaysOfWeekGrpc ToEditPropertyDaysOfWeekGrpc(this EditPropertyValue<ReadOnlyMemory<DayOfWeek>> value);
    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(this EditPropertyValue<uint> value);
    public static partial UpdateOrderIndexToDoItemOptionsGrpc ToUpdateOrderIndexToDoItemOptionsGrpc(this UpdateOrderIndexToDoItemOptions value);
    public static partial UpdateOrderIndexToDoItemOptions ToUpdateOrderIndexToDoItemOptions(this UpdateOrderIndexToDoItemOptionsGrpc value);
    public static partial DayOfYearGrpc ToDayOfYearGrpc(this DayOfYear value);
    public static partial DayOfYear ToDayOfYear(this DayOfYearGrpc value);
    public static partial ToDoItemTypeGrpc ToToDoItemTypeGrpc(this ToDoItemType value);
    public static partial ToDoItemType ToToDoItemType(this ToDoItemTypeGrpc value);
    public static partial ToDoItemIsCanGrpc ToToDoItemIsCanGrpc(this ToDoItemIsCan value);
    public static partial ToDoItemIsCan ToToDoItemIsCan(this ToDoItemIsCanGrpc value);
    public static partial ToDoItemToStringOptionsGrpc ToToDoItemToStringOptionsGrpc(this ToDoItemToStringOptions value);
    public static partial ToDoItemToStringOptions ToToDoItemToStringOptions(this ToDoItemToStringOptionsGrpc value);
    public static partial ToDoSelectorItemGrpc ToToDoSelectorItemGrpc(this ToDoSelectorItem value);
    public static partial ReadOnlyMemory<ToDoSelectorItemGrpc> ToToDoSelectorItemGrpc(this ReadOnlyMemory<ToDoSelectorItem> value);
    public static partial ToDoSelectorItem ToToDoSelectorItem(this ToDoSelectorItemGrpc value);
    public static partial ReadOnlyMemory<ToDoSelectorItem> ToToDoSelectorItem(this RepeatedField<ToDoSelectorItemGrpc> value);
    public static partial ReadOnlyMemory<FullToDoItem> ToFullToDoItem(this RepeatedField<FullToDoItemGrpc> value);
    public static partial RepeatedField<FullToDoItemGrpc> ToFullToDoItemGrpc(this ReadOnlyMemory<FullToDoItem> value);
    public static partial ToDoShortItem ToToDoShortItem(this ToDoShortItemGrpc value);
    public static partial RepeatedField<GetToStringItemGrpc> ToGetToStringItemGrpc(this ReadOnlyMemory<GetToStringItem> value);
    public static partial ReadOnlyMemory<GetToStringItem> ToGetToStringItem(this RepeatedField<GetToStringItemGrpc> value);
    public static partial ReadOnlyMemory<ToDoItemStatus> ToToDoItemStatus(this RepeatedField<ToDoItemStatusGrpc> value);
    public static partial GetReply ToGetReply(this ToDoResponse value);
    public static partial ToDoResponse ToToDoResponse(this GetReply value);
    public static partial LeafItem ToLeafItem(this LeafItemGrpc value);
    public static partial LeafItemGrpc ToLeafItemGrpc(this LeafItem value);
    public static partial ParentItem ToParentItem(this ParentItemGrpc value);
    public static partial ParentItemGrpc ToParentItemGrpc(this ParentItem value);
    private static partial ReadOnlyMemory<Guid> ToGuid(RepeatedField<ByteString> value);
    
    public static GetRequest ToGetRequest(this GetToDo value)
    {
        var result = new GetRequest
        {
            SearchText = value.SearchText,
            IsBookmarkItems = value.IsBookmarkItems,
            IsFavoriteItems = value.IsFavoriteItems,
            IsRootItems = value.IsRootItems,
            IsSelectorItems = value.IsSelectorItems,
            IsTodayItems = value.IsTodayItems,
            IsCurrentActiveItem = value.IsCurrentActiveItem,
        };
        
        result.Items.AddRange(value.Items.ToByteString().ToArray());
        result.ActiveItems.AddRange(value.ActiveItems.ToByteString().ToArray());
        result.ChildrenItems.AddRange(value.ChildrenItems.ToByteString().ToArray());
        result.LeafItems.AddRange(value.LeafItems.ToByteString().ToArray());
        result.ParentItems.AddRange(value.ParentItems.ToByteString().ToArray());
        result.ToStringItems.AddRange(value.ToStringItems.ToGetToStringItemGrpc());

        return result;
    }

    public static GetToDo ToGetToDo(this GetRequest value)
    {
        var target = new GetToDo(
            value.IsSelectorItems,
            value.ToStringItems != null ? MapToGetToStringItemArray(value.ToStringItems) : throw new ArgumentNullException(nameof(value.ToStringItems)),
            value.IsCurrentActiveItem,
            value.ActiveItems != null ? MapToGuidArray(value.ActiveItems) : throw new ArgumentNullException(nameof(value.ActiveItems)),
            value.IsFavoriteItems,
            value.IsBookmarkItems,
            value.ChildrenItems != null ? MapToGuidArray(value.ChildrenItems) : throw new ArgumentNullException(nameof(value.ChildrenItems)),
            value.LeafItems != null ? MapToGuidArray(value.LeafItems) : throw new ArgumentNullException(nameof(value.LeafItems)),
            value.SearchText ?? throw new ArgumentNullException(nameof(value.SearchText)),
            value.ParentItems != null ? MapToGuidArray(value.ParentItems) : throw new ArgumentNullException(nameof(value.ParentItems)),
            value.IsTodayItems,
            value.IsRootItems,
            value.Items != null ? MapToGuidArray(value.Items) : throw new ArgumentNullException(nameof(value.Items))
        );

        return target;
    }

    public static GetToStringItem ToGetToStringItem(this GetToStringItemGrpc value)
    {
        return new(value.Ids.ToGuid(), value.Statuses.ToToDoItemStatus());
    }

    public static DayOfWeek ToDayOfWeek(this DayOfWeekGrpc value)
    {
        return (DayOfWeek)value;
    }

    public static DayOfWeekGrpc ToDayOfWeek(this DayOfWeek value)
    {
        return (DayOfWeekGrpc)value;
    }

    public static TypeOfPeriodicity ToTypeOfPeriodicity(this TypeOfPeriodicityGrpc value)
    {
        return (TypeOfPeriodicity)value;
    }

    public static TypeOfPeriodicityGrpc ToTypeOfPeriodicityGrpc(this TypeOfPeriodicity value)
    {
        return (TypeOfPeriodicityGrpc)value;
    }

    public static DescriptionType ToDescriptionType(this DescriptionTypeGrpc value)
    {
        return (DescriptionType)value;
    }

    public static DescriptionTypeGrpc ToDescriptionTypeGrpc(this DescriptionType value)
    {
        return (DescriptionTypeGrpc)value;
    }

    public static ToDoItemStatus ToToDoItemStatus(this ToDoItemStatusGrpc value)
    {
        return (ToDoItemStatus)value;
    }

    public static ToDoItemStatusGrpc ToToDoItemStatusGrpc(this ToDoItemStatus value)
    {
        return (ToDoItemStatusGrpc)value;
    }

    public static ToDoItemChildrenType ToToDoItemChildrenType(this ToDoItemChildrenTypeGrpc value)
    {
        return (ToDoItemChildrenType)value;
    }

    public static ToDoItemChildrenTypeGrpc ToToDoItemChildrenTypeGrpc(this ToDoItemChildrenType value)
    {
        return (ToDoItemChildrenTypeGrpc)value;
    }

    public static FullToDoItem ToFullToDoItem(this FullToDoItemGrpc value)
    {
        return new(value.Item.ToToDoShortItem(), value.Status.ToToDoItemStatus(), value.Active.ToOptionToDoShortItem(), value.IsCan.ToToDoItemIsCan());
    }

    public static FullToDoItemGrpc ToFullToDoItemGrpc(this FullToDoItem value)
    {
        return new()
        {
            Active = value.Active.ToToDoShortItemNullableGrpc(),
            Item = value.Item.ToToDoShortItemGrpc(),
            Status = value.Status.ToToDoItemStatusGrpc(),
            IsCan = value.IsCan.ToToDoItemIsCanGrpc(),
        };
    }

    public static ToDoShortItemGrpc ToToDoShortItemGrpc(this ToDoShortItem value)
    {
        var item = new ToDoShortItemGrpc
        {
            Description = value.Description,
            Icon = value.Icon,
            Link = value.Link.TryGetValue(out var link) ? link.AbsoluteUri : string.Empty,
            Name = value.Name,
            Type = value.Type.ToToDoItemTypeGrpc(),
            DescriptionType = value.DescriptionType.ToDescriptionTypeGrpc(),
            IsBookmark = value.IsBookmark,
            IsFavorite = value.IsFavorite,
            ChildrenType = value.ChildrenType.ToToDoItemChildrenTypeGrpc(),
            MonthsOffset = value.MonthsOffset,
            YearsOffset = value.YearsOffset,
            ReferenceId = value.ReferenceId.ToByteString(),
            DaysOffset = value.DaysOffset,
            Color = value.Color,
            RemindDaysBefore = value.RemindDaysBefore,
            WeeksOffset = value.WeeksOffset,
            TypeOfPeriodicity = value.TypeOfPeriodicity.ToTypeOfPeriodicityGrpc(),
            ParentId = value.ParentId.ToByteString(),
            IsRequiredCompleteInDueDate = value.IsRequiredCompleteInDueDate,
            DueDate = value.DueDate.ToTimestamp(),
            Id = value.Id.ToByteString(),
            OrderIndex = value.OrderIndex,
        };

        item.AnnuallyDays.AddRange(value.AnnuallyDays.Select(x => x.ToDayOfYearGrpc()).ToArray());
        item.MonthlyDays.AddRange(value.MonthlyDays.Select(x => (uint)x).ToArray());
        item.WeeklyDays.AddRange(value.WeeklyDays.Select(x => (DayOfWeekGrpc)x).ToArray());

        return item;
    }

    public static AddToDoItemOptionsGrpc ToAddToDoItemOptionsGrpc(this AddToDoItemOptions value)
    {
        var add = new AddToDoItemOptionsGrpc
        {
            Description = value.Description,
            Icon = value.Icon,
            Link = value.Link.TryGetValue(out var link) ? link.AbsoluteUri : string.Empty,
            Name = value.Name,
            Type = value.Type.ToToDoItemTypeGrpc(),
            ParentId = value.ParentId.ToByteString(),
            IsBookmark = value.IsBookmark,
            IsFavorite = value.IsFavorite,
            ChildrenType = value.ChildrenType.ToToDoItemChildrenTypeGrpc(),
            DaysOffset = value.DaysOffset,
            DescriptionType = value.DescriptionType.ToDescriptionTypeGrpc(),
            DueDate = value.DueDate.ToTimestamp(),
            MonthsOffset = value.MonthsOffset,
            YearsOffset = value.YearsOffset,
            ReferenceId = value.ReferenceId.ToByteString(),
            WeeksOffset = value.WeeksOffset,
            TypeOfPeriodicity = (TypeOfPeriodicityGrpc)value.TypeOfPeriodicity,
            IsRequiredCompleteInDueDate = value.IsRequiredCompleteInDueDate,
            Color = value.Color,
            RemindDaysBefore = value.RemindDaysBefore,
        };

        add.AnnuallyDays.AddRange(value.AnnuallyDays.Select(x => x.ToDayOfYearGrpc()).ToArray());
        add.MonthlyDays.AddRange(value.MonthlyDays.Select(x => (uint)x).ToArray());
        add.WeeklyDays.AddRange(value.WeeklyDays.Select(x => (DayOfWeekGrpc)x).ToArray());

        return add;
    }

    public static AddToDoItemOptions ToAddToDoItemOptions(this AddToDoItemOptionsGrpc value)
    {
        return new(
            value.Name,
            value.Description,
            value.Type.ToToDoItemType(),
            value.IsBookmark,
            value.IsFavorite,
            value.DueDate.ToDateOnly(),
            value.TypeOfPeriodicity.ToTypeOfPeriodicity(),
            value.AnnuallyDays.Select(x => x.ToDayOfYear()).ToArray(),
            value.MonthlyDays.Select(x => (byte)x).ToArray(),
            value.WeeklyDays.Select(x => (DayOfWeek)x).ToArray(),
            (ushort)value.DaysOffset,
            (ushort)value.MonthsOffset,
            (ushort)value.WeeksOffset,
            (ushort)value.YearsOffset,
            value.ChildrenType.ToToDoItemChildrenType(),
            value.IsRequiredCompleteInDueDate,
            value.DescriptionType.ToDescriptionType(),
            value.Icon,
            value.Color,
            value.ReferenceId.ToOptionGuid(),
            value.ParentId.ToOptionGuid(),
            value.Link.ToOptionUri(),
            value.RemindDaysBefore
        );
    }

    public static ToDoShortItemGrpc? ToToDoShortItemNullableGrpc(this OptionStruct<ToDoShortItem> value)
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
            value.DescriptionType.IsEdit ? new(value.DescriptionType.Value.ToDescriptionType()) : new(),
            value.ReferenceId.IsEdit ? new(value.ReferenceId.Value.ToOptionGuid()) : new(),
            value.AnnuallyDays.IsEdit ? new(value.AnnuallyDays.Value.Select(x => x.ToDayOfYear()).ToArray()) : new(),
            value.MonthlyDays.IsEdit ? new(value.MonthlyDays.Value.Select(x => (byte)x).ToArray()) : new(),
            value.ChildrenType.IsEdit ? new(value.ChildrenType.Value.ToToDoItemChildrenType()) : new(),
            value.DueDate.IsEdit ? new(value.DueDate.Value.ToDateOnly()) : new(),
            value.DaysOffset.IsEdit ? new((ushort)value.DaysOffset.Value) : new(),
            value.MonthsOffset.IsEdit ? new((ushort)value.MonthsOffset.Value) : new(),
            value.WeeksOffset.IsEdit ? new((ushort)value.WeeksOffset.Value) : new(),
            value.YearsOffset.IsEdit ? new((ushort)value.YearsOffset.Value) : new(),
            value.IsRequiredCompleteInDueDate.IsEdit ? new(value.IsRequiredCompleteInDueDate.Value) : new(),
            value.TypeOfPeriodicity.IsEdit ? new(value.TypeOfPeriodicity.Value.ToTypeOfPeriodicity()) : new(),
            value.WeeklyDays.IsEdit ? new(value.WeeklyDays.Value.Select(x => x.ToDayOfWeek()).ToArray()) : new(),
            value.IsBookmark.IsEdit ? new(value.IsBookmark.Value) : new(),
            value.Icon.IsEdit ? new(value.Icon.Value) : new(),
            value.Color.IsEdit ? new(value.Color.Value) : new(),
            value.RemindDaysBefore.IsEdit ? new(value.RemindDaysBefore.Value) : new()
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
            IsRequiredCompleteInDueDate = value.IsRequiredCompleteInDueDate.ToEditPropertyBooleanGrpc(),
            Color = value.Color.ToEditPropertyStringGrpc(),
            RemindDaysBefore = value.RemindDaysBefore.ToEditPropertyUInt32Grpc(),
        };

        edit.Ids.AddRange(value.Ids.ToByteString().ToArray());

        return edit;
    }

    public static EditPropertyTimestampGrpc ToEditPropertyTimestampGrpc(this EditPropertyValue<DateOnly> value)
    {
        if (value.IsEdit)
        {
            return new()
            {
                IsEdit = true,
                Value = value.Value.ToTimestamp(),
            };
        }

        return new()
        {
            IsEdit = false,
            Value = null,
        };
    }

    public static EditPropertyBytesGrpc ToEditPropertyBytesGrpc(this EditPropertyValue<OptionStruct<Guid>> value)
    {
        if (!value.IsEdit)
        {
            return new()
            {
                IsEdit = false,
                Value = ByteString.Empty,
            };
        }

        return new()
        {
            IsEdit = true,
            Value = value.Value.ToByteString(),
        };
    }

    public static EditPropertyStringGrpc ToEditPropertyStringGrpc(this EditPropertyValue<Option<Uri>> value)
    {
        if (!value.IsEdit)
        {
            return new()
            {
                IsEdit = false,
                Value = string.Empty,
            };
        }

        if (value.Value.TryGetValue(out var link))
        {
            return new()
            {
                IsEdit = true,
                Value = link.AbsoluteUri,
            };
        }

        return new()
        {
            IsEdit = true,
            Value = string.Empty,
        };
    }

    public static EditPropertyStringGrpc ToEditPropertyStringGrpc(this EditPropertyValue<string> value)
    {
        if (value.IsEdit)
        {
            return new()
            {
                IsEdit = true,
                Value = value.Value,
            };
        }

        return new()
        {
            IsEdit = false,
            Value = string.Empty,
        };
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