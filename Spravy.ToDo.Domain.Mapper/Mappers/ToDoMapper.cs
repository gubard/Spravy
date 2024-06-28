using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ToDoMapper
{
    public static partial AnnuallyPeriodicity ToAnnuallyPeriodicity(
        this AnnuallyPeriodicityGrpc value
    );

    public static partial AnnuallyPeriodicityGrpc ToAnnuallyPeriodicityGrpc(
        this AnnuallyPeriodicity value
    );

    public static partial MonthlyPeriodicity ToMonthlyPeriodicity(
        this MonthlyPeriodicityGrpc value
    );

    public static partial MonthlyPeriodicityGrpc ToMonthlyPeriodicityGrpc(
        this MonthlyPeriodicity value
    );

    public static partial ResetToDoItemOptions ToResetToDoItemOptions(
        this ResetToDoItemRequest value
    );

    public static partial ResetToDoItemRequest ToResetToDoItemRequest(
        this ResetToDoItemOptions value
    );

    public static partial WeeklyPeriodicity ToWeeklyPeriodicity(this WeeklyPeriodicityGrpc value);

    public static partial WeeklyPeriodicityGrpc ToWeeklyPeriodicityGrpc(
        this WeeklyPeriodicity value
    );

    public static partial MonthlyPeriodicity ToMonthlyPeriodicity(
        this GetMonthlyPeriodicityReply value
    );

    public static partial GetMonthlyPeriodicityReply ToGetMonthlyPeriodicityReply(
        this MonthlyPeriodicity value
    );

    public static partial DayOfWeek ToDayOfWeekGrpc(this DayOfWeekGrpc value);

    public static partial DayOfWeekGrpc ToDayOfWeek(this DayOfWeek value);

    public static partial DescriptionType ToDescriptionType(this DescriptionTypeGrpc value);

    public static partial DescriptionTypeGrpc ToDescriptionTypeGrpc(this DescriptionType value);

    public static partial ToDoItem ToToDoItem(this ToDoItemGrpc value);

    public static partial ToDoItemGrpc ToToDoItemGrpc(this ToDoItem value);

    public static partial ReadOnlyMemory<ToDoItemGrpc> ToToDoItemGrpc(
        this ReadOnlyMemory<ToDoItem> value
    );

    public static partial ReadOnlyMemory<ToDoItem> ToToDoItem(this IEnumerable<ToDoItemGrpc> value);

    public static partial ToDoItem ToToDoItem(this GetToDoItemReply value);

    public static partial GetToDoItemReply ToGetToDoItemReply(this ToDoItem value);

    public static partial ValueToDoItemSettings ToValueToDoItemSettings(
        this GetValueToDoItemSettingsReply value
    );

    public static partial ToDoShortItemGrpc ToToDoShortItemGrpc(this ToDoShortItem value);

    public static partial ToDoShortItem ToToDoShortItem(this ToDoShortItemGrpc value);

    public static partial ReadOnlyMemory<ToDoShortItemGrpc> ToToDoShortItemGrpc(
        this ReadOnlyMemory<ToDoShortItem> value
    );

    public static partial ReadOnlyMemory<ToDoShortItem> ToToDoShortItem(
        this IEnumerable<ToDoShortItemGrpc> value
    );

    public static partial AddRootToDoItemRequest ToAddRootToDoItemRequest(
        this AddRootToDoItemOptions value
    );

    public static partial AddRootToDoItemOptions ToAddRootToDoItemOptions(
        this AddRootToDoItemRequest value
    );

    public static partial AddToDoItemRequest ToAddToDoItemRequest(this AddToDoItemOptions value);

    public static partial AddToDoItemOptions ToAddToDoItemOptions(this AddToDoItemRequest value);

    public static partial ActiveToDoItemGrpc ToActiveToDoItemGrpc(this ActiveToDoItem value);

    public static partial ActiveToDoItem ToActiveToDoItem(this ActiveToDoItemGrpc value);

    public static partial DayOfYearGrpc ToDayOfYearGrpc(this DayOfYear value);

    public static partial DayOfYear ToDayOfYear(this DayOfYearGrpc value);

    public static partial ToDoItemTypeGrpc ToToDoItemTypeGrpc(this ToDoItemType value);

    public static partial ToDoItemType ToToDoItemType(this ToDoItemTypeGrpc value);

    public static partial ToDoItemIsCanGrpc ToToDoItemIsCanGrpc(this ToDoItemIsCan value);

    public static partial ToDoItemIsCan ToToDoItemIsCan(this ToDoItemIsCanGrpc value);

    public static partial GetAnnuallyPeriodicityReply ToGetAnnuallyPeriodicityReply(
        this AnnuallyPeriodicity value
    );

    public static partial AnnuallyPeriodicity ToAnnuallyPeriodicity(
        this GetAnnuallyPeriodicityReply value
    );

    public static partial ToDoItemToStringRequest ToToDoItemToStringRequest(
        this ToDoItemToStringOptions value
    );

    public static partial ToDoItemToStringOptions ToToDoItemToStringOptions(
        this ToDoItemToStringRequest value
    );

    public static partial ToDoSelectorItemGrpc ToToDoSelectorItemGrpc(this ToDoSelectorItem value);

    public static partial ReadOnlyMemory<ToDoSelectorItemGrpc> ToToDoSelectorItemGrpc(
        this ReadOnlyMemory<ToDoSelectorItem> value
    );

    public static partial ToDoSelectorItem ToToDoSelectorItem(this ToDoSelectorItemGrpc value);

    public static partial ReadOnlyMemory<ToDoSelectorItem> ToToDoSelectorItem(
        this IEnumerable<ToDoSelectorItemGrpc> value
    );

    public static partial GetWeeklyPeriodicityReply ToGetWeeklyPeriodicityReply(
        this WeeklyPeriodicity value
    );

    public static partial WeeklyPeriodicity ToWeeklyPeriodicity(
        this GetWeeklyPeriodicityReply value
    );

    public static partial PlannedToDoItemSettings ToPlannedToDoItemSettings(
        this GetPlannedToDoItemSettingsReply value
    );

    public static ActiveToDoItemGrpc? ToActiveToDoItemGrpc(this OptionStruct<ActiveToDoItem> value)
    {
        if (value.TryGetValue(out var item))
        {
            return item.ToActiveToDoItemGrpc();
        }

        return null;
    }

    public static OptionStruct<ActiveToDoItem> ToOptionActiveToDoItem(
        this ActiveToDoItemGrpc? value
    )
    {
        if (value is null)
        {
            return new();
        }

        return new(value.ToActiveToDoItem());
    }

    public static DailyPeriodicity ToDailyPeriodicity(this DailyPeriodicityGrpc value)
    {
        return new();
    }

    public static DailyPeriodicityGrpc ToDailyPeriodicityGrpc(this DailyPeriodicity value)
    {
        return new();
    }

    public static partial UpdateToDoItemOrderIndexRequest ToUpdateToDoItemOrderIndexRequest(
        this UpdateOrderIndexToDoItemOptions value
    );

    public static partial UpdateOrderIndexToDoItemOptions ToUpdateOrderIndexToDoItemOptions(
        this UpdateToDoItemOrderIndexRequest value
    );

    public static partial GetPlannedToDoItemSettingsReply ToGetPlannedToDoItemSettingsReply(
        this PlannedToDoItemSettings value
    );

    public static partial GetValueToDoItemSettingsReply ToGetValueToDoItemSettingsReply(
        this ValueToDoItemSettings value
    );

    public static partial PeriodicityToDoItemSettings ToPeriodicityToDoItemSettings(
        this GetPeriodicityToDoItemSettingsReply value
    );

    public static partial GetPeriodicityToDoItemSettingsReply ToGetPeriodicityToDoItemSettingsReply(
        this PeriodicityToDoItemSettings value
    );

    public static partial PeriodicityOffsetToDoItemSettings ToPeriodicityOffsetToDoItemSettings(
        this GetPeriodicityOffsetToDoItemSettingsReply value
    );

    public static partial GetPeriodicityOffsetToDoItemSettingsReply ToGetPeriodicityOffsetToDoItemSettingsReply(
        this PeriodicityOffsetToDoItemSettings value
    );

    public static partial ReferenceToDoItemSettings ToReferenceToDoItemSettings(
        this GetReferenceToDoItemSettingsReply value
    );

    public static partial GetReferenceToDoItemSettingsReply ToGetReferenceToDoItemSettingsReply(
        this ReferenceToDoItemSettings value
    );

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
