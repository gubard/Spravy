using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Domain.Mapper.Profiles;

public class SpravyToDoProfile : Profile
{
    public SpravyToDoProfile()
    {
        CreateMap<AnnuallyPeriodicityGrpc, AnnuallyPeriodicity>();
        CreateMap<AnnuallyPeriodicity, AnnuallyPeriodicityGrpc>();
        CreateMap<MonthlyPeriodicityGrpc, MonthlyPeriodicity>();
        CreateMap<MonthlyPeriodicity, MonthlyPeriodicityGrpc>();
        CreateMap<WeeklyPeriodicityGrpc, WeeklyPeriodicity>();
        CreateMap<WeeklyPeriodicity, WeeklyPeriodicityGrpc>();
        CreateMap<GetWeeklyPeriodicityReply, WeeklyPeriodicity>();
        CreateMap<WeeklyPeriodicity, GetWeeklyPeriodicityReply>();
        CreateMap<GetMonthlyPeriodicityReply, MonthlyPeriodicity>();
        CreateMap<MonthlyPeriodicity, GetMonthlyPeriodicityReply>();
        CreateMap<GetAnnuallyPeriodicityReply, AnnuallyPeriodicity>();
        CreateMap<AnnuallyPeriodicity, GetAnnuallyPeriodicityReply>();
        CreateMap<ToDoItem, ToDoItemGrpc>();
        CreateMap<ToDoItemGrpc, ToDoItem>();
        CreateMap<ToDoItem, GetToDoItemReply>();
        CreateMap<GetToDoItemReply, ToDoItem>();
        CreateMap<PeriodicityOffsetToDoItemSettings, GetPeriodicityOffsetToDoItemSettingsReply>();
        CreateMap<GetPeriodicityOffsetToDoItemSettingsReply, PeriodicityOffsetToDoItemSettings>();
        CreateMap<PeriodicityToDoItemSettings, GetPeriodicityToDoItemSettingsReply>();
        CreateMap<GetPeriodicityToDoItemSettingsReply, PeriodicityToDoItemSettings>();
        CreateMap<ValueToDoItemSettings, GetValueToDoItemSettingsReply>();
        CreateMap<GetValueToDoItemSettingsReply, ValueToDoItemSettings>();
        CreateMap<PlannedToDoItemSettings, GetPlannedToDoItemSettingsReply>();
        CreateMap<GetPlannedToDoItemSettingsReply, PlannedToDoItemSettings>();
        CreateMap<ToDoShortItem, ToDoShortItemGrpc>();
        CreateMap<ToDoShortItemGrpc, ToDoShortItem>();
        CreateMap<AddRootToDoItemOptions, AddRootToDoItemRequest>();
        CreateMap<AddToDoItemOptions, AddToDoItemRequest>();
        CreateMap<UpdateOrderIndexToDoItemOptions, UpdateToDoItemOrderIndexRequest>();
        CreateMap<ActiveToDoItem?, ActiveToDoItemGrpc?>()
            .ConvertUsing((x, _, context) => x.HasValue ? context.Mapper.Map<ActiveToDoItemGrpc>(x.Value) : null);
        CreateMap<ActiveToDoItem, ActiveToDoItemGrpc>();
        CreateMap<DateOnly, Timestamp>().ConvertUsing(x => Timestamp.FromDateTime(x.ToDateTime(DateTimeKind.Utc)));
        CreateMap<Timestamp, DateOnly>().ConvertUsing(x => x.ToDateTime().ToDateOnly());
        CreateMap<DateOnly, DateTimeOffset>().ConvertUsing(x => x.ToDateTimeOffset());
        CreateMap<DateTimeOffset, DateOnly>().ConvertUsing(x => x.Date.ToDateOnly());
        CreateMap<DailyPeriodicity, DailyPeriodicityGrpc>();
        CreateMap<AddToDoItemRequest, AddToDoItemOptions>();
        CreateMap<AddRootToDoItemRequest, AddRootToDoItemOptions>();
        CreateMap<TimeSpan, Duration>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<TimeSpan?, Duration?>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<Duration?, TimeSpan?>().ConvertUsing(x => DurationToTimeSpanNull(x));
        CreateMap<Duration, TimeSpan>().ConvertUsing(x => DurationToTimeSpan(x));
        CreateMap<DayOfYearGrpc, DayOfYear>().ConstructUsing(x => new((byte)x.Day, (byte)x.Month));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToNullableDateTimeOffset(x));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoSelectorItem, ToDoSelectorItemGrpc>();
        CreateMap<ToDoItemType, ToDoItemTypeGrpc>();
        CreateMap<ToDoItemTypeGrpc, ToDoItemType>();
        CreateMap<ToDoItemIsCan, ToDoItemIsCanGrpc>();
        CreateMap<ToDoItemIsCanGrpc, ToDoItemIsCan>();
        CreateMap<string?, Uri?>().ConvertUsing(x => x.IsNullOrWhiteSpace() ? null : new Uri(x));
        CreateMap<Uri?, string?>().ConvertUsing(x => x == null ? string.Empty : x.AbsoluteUri);

        CreateMap<ToDoItemToStringRequest, ToDoItemToStringOptions>()
            .ConstructUsing(
                (x, context) => new(
                    context.Mapper.Map<IEnumerable<ToDoItemStatus>>(x.Statuses),
                    context.Mapper.Map<Guid>(x.Id)
                )
            );

        CreateMap<ToDoItemToStringOptions, ToDoItemToStringRequest>()
            .ConvertUsing(
                (x, _, context) =>
                {
                    var request = new ToDoItemToStringRequest
                    {
                        Id = context.Mapper.Map<ByteString>(x.Id)
                    };

                    request.Statuses.AddRange(
                        context.Mapper.Map<IEnumerable<ToDoItemStatusGrpc>>(x.Statuses.ToArray())
                    );

                    return request;
                }
            );

        CreateMap<MonthlyPeriodicityGrpc, MonthlyPeriodicity>()
            .ConvertUsing((source, _, _) => new(source.Days.ToByteArray()));

        CreateMap<ToDoSelectorItemGrpc, ToDoSelectorItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => new(
                    resolutionContext.Mapper.Map<Guid>(source.Id),
                    source.Name,
                    resolutionContext.Mapper.Map<ToDoSelectorItem[]>(source.Children)
                )
            );

        CreateMap<AnnuallyPeriodicityGrpc, AnnuallyPeriodicity>()
            .ConvertUsing(
                (source, _, context) =>
                    new(context.Mapper.Map<IEnumerable<DayOfYear>>(source.Days))
            );

        CreateMap<DayOfYear, DayOfYearGrpc>()
            .ConvertUsing(
                (source, _, _) => new()
                {
                    Day = source.Day,
                    Month = source.Month,
                }
            );

        CreateMap<MonthlyPeriodicity, MonthlyPeriodicityGrpc>()
            .ConvertUsing(
                (source, _, _) => new()
                {
                    Days = ByteString.CopyFrom(Enumerable.ToArray(source.Days))
                }
            );

        CreateMap<AnnuallyPeriodicity, AnnuallyPeriodicityGrpc>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    var result = new AnnuallyPeriodicityGrpc();
                    result.Days.AddRange(source.Days.Select(x => resolutionContext.Mapper.Map<DayOfYearGrpc>(x)));

                    return result;
                }
            );

        CreateMap<ToDoSubItemPeriodicityGrpc, IPeriodicity>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.PeriodicityCase switch
                {
                    ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.None =>
                        throw new ArgumentOutOfRangeException(),
                    ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Daily => new DailyPeriodicity(),
                    ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Weekly => new WeeklyPeriodicity(
                        source.Weekly.Days.Select(x => (DayOfWeek)x)
                    ),
                    ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Monthly => new MonthlyPeriodicity(
                        source.Monthly.ToByteArray()
                    ),
                    ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Annually => new AnnuallyPeriodicity(
                        resolutionContext.Mapper.Map<IEnumerable<DayOfYear>>(source.Annually.Days)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<ToDoItemPeriodicityGrpc, IPeriodicity>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.PeriodicityCase switch
                {
                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.None =>
                        throw new ArgumentOutOfRangeException(),
                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Daily => new DailyPeriodicity(),
                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Weekly => new WeeklyPeriodicity(
                        source.Weekly.Days.Select(x => (DayOfWeek)x)
                    ),
                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Monthly => new MonthlyPeriodicity(
                        Enumerable.ToArray(source.Monthly.Days)
                    ),
                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Annually => new AnnuallyPeriodicity(
                        resolutionContext.Mapper.Map<IEnumerable<DayOfYear>>(source.Annually.Days)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<Guid?, ByteString>()
            .ConvertUsing(x => x.HasValue ? ByteString.CopyFrom(x.Value.ToByteArray()) : ByteString.Empty);

        CreateMap<ByteString, Guid>()
            .ConstructUsing(x => new(x.ToByteArray()));

        CreateMap<ByteString, Guid?>()
            .ConvertUsing(x => x.IsEmpty ? null : new Guid(x.ToByteArray()));

        CreateMap<ActiveToDoItemGrpc, ActiveToDoItem?>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    if (source is null)
                    {
                        return null;
                    }

                    return new ActiveToDoItem(resolutionContext.Mapper.Map<Guid>(source.Id), source.Name);
                }
            );

        CreateMap<DateTimeOffset, DateTimeOffsetGrpc>()
            .ConvertUsing(
                (source, _, _) => new()
                {
                    Date = ToTimestamp(source),
                    Offset = OffsetToDuration(source),
                }
            );

        CreateMap<DateTimeOffset?, DateTimeOffsetGrpc>()
            .ConvertUsing(
                x => new()
                {
                    Date = ToTimestamp(x),
                    Offset = OffsetToDuration(x)
                }
            );

        CreateMap<UpdateToDoItemOrderIndexRequest, UpdateOrderIndexToDoItemOptions>()
            .ConstructUsing(
                (src, res) => new(
                    res.Mapper.Map<Guid>(src.Id),
                    res.Mapper.Map<Guid>(src.TargetId),
                    src.IsAfter
                )
            );
    }

    private DateTimeOffset ToDateTimeOffset(DateTimeOffsetGrpc grpc)
    {
        var date = grpc.Date.ToDateTimeOffset();

        return new(
            date.Year,
            date.Month,
            date.Day,
            date.Hour,
            date.Minute,
            date.Second,
            grpc.Offset.ToTimeSpan()
        );
    }

    private DateTimeOffset? ToNullableDateTimeOffset(DateTimeOffsetGrpc grpc)
    {
        if (grpc.Date is null)
        {
            return null;
        }

        var date = grpc.Date.ToDateTimeOffset();

        return new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            date.Hour,
            date.Minute,
            date.Second,
            grpc.Offset.ToTimeSpan()
        );
    }

    private Duration? OffsetToDuration(DateTimeOffset? date)
    {
        return date?.Offset.ToDuration();
    }

    private Duration? TimeSpanToDuration(TimeSpan? span)
    {
        return span is null ? null : Duration.FromTimeSpan(span.Value);
    }

    private Duration TimeSpanToDuration(TimeSpan span)
    {
        return Duration.FromTimeSpan(span);
    }

    private TimeSpan? DurationToTimeSpanNull(Duration? duration)
    {
        return duration?.ToTimeSpan();
    }

    private TimeSpan DurationToTimeSpan(Duration duration)
    {
        return duration.ToTimeSpan();
    }

    private Timestamp? ToTimestamp(DateTimeOffset? date)
    {
        if (!date.HasValue)
        {
            return null;
        }

        return date.Value.Add(date.Value.Offset).ToTimestamp();
    }

    private DateTimeOffset? ToDateTimeOffset(Timestamp? timestamp)
    {
        return timestamp?.ToDateTimeOffset();
    }
}