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
        CreateMap<ToDoItemParent, ToDoItemParentGrpc>();
        CreateMap<ToDoShortItem, ToDoShortItemGrpc>();
        CreateMap<ToDoShortItemGrpc, ToDoShortItem>();
        CreateMap<ToDoItemParentGrpc, ToDoItemParent>();
        CreateMap<AddRootToDoItemOptions, AddRootToDoItemRequest>();
        CreateMap<ToDoSubItemValue, ToDoSubItemValueGrpc>();
        CreateMap<ToDoSubItemPeriodicityOffset, ToDoSubItemPeriodicityOffsetGrpc>();
        CreateMap<ToDoItemGroup, ToDoItemGroupGrpc>();
        CreateMap<ToDoItemValue, ToDoItemValueGrpc>();
        CreateMap<ToDoSubItemGroup, ToDoSubItemGroupGrpc>();
        CreateMap<AddToDoItemOptions, AddToDoItemRequest>();
        CreateMap<UpdateOrderIndexToDoItemOptions, UpdateToDoItemOrderIndexRequest>();
        CreateMap<ActiveToDoItem?, ActiveToDoItemGrpc>();
        CreateMap<ActiveToDoItem, ActiveToDoItemGrpc>();
        CreateMap<ToDoItemPlanned, ToDoItemPlannedGrpc>();
        CreateMap<ToDoSubItemPlanned, ToDoSubItemPlannedGrpc>();
        CreateMap<ToDoItemCircle, ToDoItemCircleGrpc>();
        CreateMap<ToDoSubItemCircle, ToDoSubItemCircleGrpc>();
        CreateMap<ToDoSubItemPeriodicity, ToDoSubItemPeriodicityGrpc>();
        CreateMap<DailyPeriodicity, DailyPeriodicityGrpc>();
        CreateMap<ToDoItemPeriodicityOffset, ToDoItemPeriodicityOffsetGrpc>();
        CreateMap<AddToDoItemRequest, AddToDoItemOptions>();
        CreateMap<AddRootToDoItemRequest, AddRootToDoItemOptions>();
        CreateMap<TimeSpan, Duration>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<TimeSpan?, Duration?>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<Duration?, TimeSpan?>().ConvertUsing(x => DurationToTimeSpan(x));
        CreateMap<DayOfYearGrpc, DayOfYear>().ConstructUsing(x => new((byte)x.Day, (byte)x.Month));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToNullableDateTimeOffset(x));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoSelectorItem, ToDoSelectorItemGrpc>();
        CreateMap<ToDoItemType, ToDoItemTypeGrpc>();
        CreateMap<ToDoItemTypeGrpc, ToDoItemType>();
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

        CreateMap<WeeklyPeriodicityGrpc, WeeklyPeriodicity>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                    new(resolutionContext.Mapper.Map<IEnumerable<DayOfWeek>>(source.Days))
            );

        CreateMap<DayOfYear, DayOfYearGrpc>()
            .ConvertUsing(
                (source, _, _) => new()
                {
                    Day = source.Day,
                    Month = source.Month,
                }
            );

        CreateMap<WeeklyPeriodicity, WeeklyPeriodicityGrpc>()
            .ConvertUsing(
                (source, _, _) =>
                {
                    var result = new WeeklyPeriodicityGrpc();
                    result.Days.AddRange(source.Days.Select(x => (DayOfWeekGrpc)x));

                    return result;
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

        CreateMap<ToDoItemPeriodicity, ToDoItemPeriodicityGrpc>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    var result = new ToDoItemPeriodicityGrpc
                    {
                        DueDate = resolutionContext.Mapper.Map<DateTimeOffsetGrpc>(source.DueDate),
                        ChildrenType = (ToDoItemChildrenTypeGrpc)source.ChildrenType,
                    };

                    switch (source.Periodicity)
                    {
                        case AnnuallyPeriodicity annuallyPeriodicity:
                            result.Annually =
                                resolutionContext.Mapper.Map<AnnuallyPeriodicityGrpc>(annuallyPeriodicity);

                            break;
                        case DailyPeriodicity dailyPeriodicity:
                            result.Daily = resolutionContext.Mapper.Map<DailyPeriodicityGrpc>(dailyPeriodicity);

                            break;
                        case MonthlyPeriodicity monthlyPeriodicity:
                            result.Monthly = resolutionContext.Mapper.Map<MonthlyPeriodicityGrpc>(monthlyPeriodicity);

                            break;
                        case WeeklyPeriodicity weeklyPeriodicity:
                            result.Weekly = resolutionContext.Mapper.Map<WeeklyPeriodicityGrpc>(weeklyPeriodicity);

                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }

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
                (source, _, resolutionContext) => source is null
                    ? null
                    : new ActiveToDoItem(resolutionContext.Mapper.Map<Guid>(source.Id), source.Name)
            );

        CreateMap<DateTimeOffset, DateTimeOffsetGrpc>()
            .ConvertUsing(
                (source, _, _) => new()
                {
                    Date = ToTimestamp(source),
                    Offset = OffsetToDuration(source),
                }
            );

        CreateMap<IToDoItem, ToDoItemGrpc>()
            .ConvertUsing(
                (source, _, context) =>
                {
                    var result = new ToDoItemGrpc
                    {
                        Description = source.Description,
                        Id = context.Mapper.Map<ByteString>(source.Id),
                        Name = source.Name,
                        IsFavorite = source.IsFavorite,
                        Link = context.Mapper.Map<string>(source.Link) ?? string.Empty,
                    };

                    result.Parents.AddRange(
                        context.Mapper.Map<IEnumerable<ToDoItemParentGrpc>>(source.Parents)
                    );

                    result.Items.AddRange(context.Mapper.Map<IEnumerable<ToDoSubItemGrpc>>(source.Items));

                    switch (source)
                    {
                        case ToDoItemGroup group:
                            result.Group = context.Mapper.Map<ToDoItemGroupGrpc>(group);

                            break;
                        case ToDoItemPeriodicity periodicity:
                            result.Periodicity =
                                context.Mapper.Map<ToDoItemPeriodicityGrpc>(periodicity);

                            break;
                        case ToDoItemPlanned planned:
                            result.Planned = context.Mapper.Map<ToDoItemPlannedGrpc>(planned);

                            break;
                        case ToDoItemValue value:
                            result.Value = context.Mapper.Map<ToDoItemValueGrpc>(value);

                            break;
                        case ToDoItemPeriodicityOffset periodicityOffset:
                            result.PeriodicityOffset =
                                context.Mapper.Map<ToDoItemPeriodicityOffsetGrpc>(periodicityOffset);

                            break;

                        case ToDoItemCircle circle:
                            result.Circle = context.Mapper.Map<ToDoItemCircleGrpc>(circle);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(source));
                    }

                    return result;
                }
            );

        CreateMap<ToDoItemGrpc, IToDoItem>()
            .ConvertUsing(
                (source, _, context) => source.ParametersCase switch
                {
                    ToDoItemGrpc.ParametersOneofCase.Circle => new ToDoItemCircle(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        context.Mapper.Map<IToDoSubItem[]>(source.Items),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.Circle.IsCompleted,
                        source.Description,
                        source.IsFavorite,
                        (ToDoItemChildrenType)source.Circle.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Value => new ToDoItemValue(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        context.Mapper.Map<IToDoSubItem[]>(source.Items),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.Value.IsCompleted,
                        source.Description,
                        source.IsFavorite,
                        (ToDoItemChildrenType)source.Value.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Group => new ToDoItemGroup(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Items.Select(x => context.Mapper.Map<IToDoSubItem>(x)).ToArray(),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Planned => new ToDoItemPlanned(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        context.Mapper.Map<IToDoSubItem[]>(source.Items),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsFavorite,
                        context.Mapper.Map<DateTimeOffset>(source.Planned.DueDate),
                        source.Planned.IsCompleted,
                        (ToDoItemChildrenType)source.Planned.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Periodicity => new ToDoItemPeriodicity(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        context.Mapper.Map<IToDoSubItem[]>(source.Items),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsFavorite,
                        context.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        context.Mapper.Map<IPeriodicity>(source.Periodicity),
                        (ToDoItemChildrenType)source.Periodicity.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.PeriodicityOffset => new ToDoItemPeriodicityOffset(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        context.Mapper.Map<IToDoSubItem[]>(source.Items),
                        context.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsFavorite,
                        (ushort)source.PeriodicityOffset.DaysOffset,
                        (ushort)source.PeriodicityOffset.MonthsOffset,
                        (ushort)source.PeriodicityOffset.WeeksOffset,
                        (ushort)source.PeriodicityOffset.YearsOffset,
                        context.Mapper.Map<DateTimeOffset>(source.PeriodicityOffset.DueDate),
                        (ToDoItemChildrenType)source.PeriodicityOffset.ChildrenType,
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoItemGrpc.ParametersOneofCase.None => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<IToDoSubItem, ToDoSubItemGrpc>()
            .ConvertUsing(
                (source, _, context) =>
                {
                    var result = new ToDoSubItemGrpc
                    {
                        Status = (ToDoItemStatusGrpc)source.Status,
                        Description = source.Description,
                        Id = context.Mapper.Map<ByteString>(source.Id),
                        Name = source.Name,
                        OrderIndex = source.OrderIndex,
                        IsFavorite = source.IsFavorite,
                        Active = context.Mapper.Map<ActiveToDoItemGrpc>(source.Active),
                        Link = context.Mapper.Map<string>(source.Link) ?? string.Empty,
                    };

                    switch (source)
                    {
                        case ToDoSubItemGroup group:
                            result.Group = context.Mapper.Map<ToDoSubItemGroupGrpc>(group);

                            break;
                        case ToDoSubItemPeriodicity periodicity:
                            result.Periodicity =
                                context.Mapper.Map<ToDoSubItemPeriodicityGrpc>(periodicity);

                            break;
                        case ToDoSubItemPlanned planned:
                            result.Planned = context.Mapper.Map<ToDoSubItemPlannedGrpc>(planned);

                            break;
                        case ToDoSubItemValue value:
                            result.Value = context.Mapper.Map<ToDoSubItemValueGrpc>(value);

                            break;
                        case ToDoSubItemCircle circle:
                            result.Circle = context.Mapper.Map<ToDoSubItemCircleGrpc>(circle);

                            break;
                        case ToDoSubItemPeriodicityOffset periodicityOffset:
                            result.PeriodicityOffset =
                                context.Mapper.Map<ToDoSubItemPeriodicityOffsetGrpc>(
                                    periodicityOffset
                                );

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(source));
                    }

                    return result;
                }
            );

        CreateMap<ToDoSubItemGrpc, IToDoSubItem>()
            .ConvertUsing(
                (source, _, context) => source.ParametersCase switch
                {
                    ToDoSubItemGrpc.ParametersOneofCase.Circle => new ToDoSubItemCircle(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Circle.IsCompleted,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.Circle.CompletedCount,
                        source.Circle.SkippedCount,
                        source.Circle.FailedCount,
                        source.IsFavorite,
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        context.Mapper.Map<DateTimeOffset?>(source.Circle.LastCompleted),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Value => new ToDoSubItemValue(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Value.IsCompleted,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.Value.CompletedCount,
                        source.Value.SkippedCount,
                        source.Value.FailedCount,
                        source.IsFavorite,
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        context.Mapper.Map<DateTimeOffset?>(source.Value.LastCompleted),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Group => new ToDoSubItemGroup(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.None => throw new ArgumentOutOfRangeException(),
                    ToDoSubItemGrpc.ParametersOneofCase.Planned => new ToDoSubItemPlanned(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        context.Mapper.Map<DateTimeOffset>(source.Planned.DueDate),
                        source.Planned.CompletedCount,
                        source.Planned.SkippedCount,
                        source.Planned.FailedCount,
                        source.Planned.IsCompleted,
                        context.Mapper.Map<DateTimeOffset?>(source.Planned.LastCompleted),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Periodicity => new ToDoSubItemPeriodicity(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        source.Periodicity.CompletedCount,
                        source.Periodicity.SkippedCount,
                        source.Periodicity.FailedCount,
                        context.Mapper.Map<DateTimeOffset?>(source.Periodicity.LastCompleted),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.PeriodicityOffset => new ToDoSubItemPeriodicityOffset(
                        context.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsFavorite,
                        context.Mapper.Map<DateTimeOffset>(source.PeriodicityOffset.DueDate),
                        context.Mapper.Map<ActiveToDoItem?>(source.Active),
                        source.PeriodicityOffset.CompletedCount,
                        source.PeriodicityOffset.SkippedCount,
                        source.PeriodicityOffset.FailedCount,
                        context.Mapper.Map<DateTimeOffset?>(source.PeriodicityOffset.LastCompleted),
                        context.Mapper.Map<Uri>(source.Link)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
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

    private TimeSpan? DurationToTimeSpan(Duration? duration)
    {
        return duration?.ToTimeSpan();
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