using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
        CreateMap<ToDoSubItemPeriodicity, ToDoSubItemPeriodicityGrpc>();
        CreateMap<DailyPeriodicity, DailyPeriodicityGrpc>();
        CreateMap<ToDoItemPeriodicityOffset, ToDoItemPeriodicityOffsetGrpc>();

        CreateMap<TimeSpan, Duration>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<TimeSpan?, Duration?>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<Duration?, TimeSpan?>().ConvertUsing(x => DurationToTimeSpan(x));
        CreateMap<DayOfYearGrpc, DayOfYear>().ConstructUsing(x => new DayOfYear((byte)x.Day, (byte)x.Month));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToNullableDateTimeOffset(x));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoSelectorItem, ToDoSelectorItemGrpc>();

        CreateMap<MonthlyPeriodicityGrpc, MonthlyPeriodicity>()
            .ConvertUsing((source, _, _) => new(source.Days.ToByteArray()));

        CreateMap<ToDoSelectorItemGrpc, ToDoSelectorItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => new ToDoSelectorItem(
                    resolutionContext.Mapper.Map<Guid>(source.Id),
                    source.Name,
                    resolutionContext.Mapper.Map<ToDoSelectorItem[]>(source.Children)
                )
            );

        CreateMap<AnnuallyPeriodicityGrpc, AnnuallyPeriodicity>()
            .ConvertUsing(
                (source, _, context) =>
                    new AnnuallyPeriodicity(context.Mapper.Map<IEnumerable<DayOfYear>>(source.Days))
            );

        CreateMap<WeeklyPeriodicityGrpc, WeeklyPeriodicity>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                    new WeeklyPeriodicity(resolutionContext.Mapper.Map<IEnumerable<DayOfWeek>>(source.Days))
            );

        CreateMap<DayOfYear, DayOfYearGrpc>()
            .ConvertUsing(
                (source, _, _) => new DayOfYearGrpc
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
                (source, _, _) => new MonthlyPeriodicityGrpc()
                {
                    Days = ByteString.CopyFrom(source.Days.ToArray())
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
                        source.Monthly.Days.ToArray()
                    ),

                    ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Annually => new AnnuallyPeriodicity(
                        resolutionContext.Mapper.Map<IEnumerable<DayOfYear>>(source.Annually.Days)
                    ),

                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<Guid?, ByteString>()
            .ConvertUsing(x => x.HasValue ? ByteString.CopyFrom(x.Value.ToByteArray()) : ByteString.Empty);
        CreateMap<AddToDoItemRequest, AddToDoItemOptions>()
            .ConstructUsing(x => new AddToDoItemOptions(new Guid(x.ParentId.ToByteArray()), x.Name));
        CreateMap<AddRootToDoItemRequest, AddRootToDoItemOptions>()
            .ConstructUsing(x => new AddRootToDoItemOptions(x.Name));
        CreateMap<ByteString, Guid>()
            .ConstructUsing(x => new Guid(x.ToByteArray()));
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
                (source, _, _) => new DateTimeOffsetGrpc
                {
                    Date = ToTimestamp(source),
                    Offset = OffsetToDuration(source),
                }
            );

        CreateMap<IToDoItem, ToDoItemGrpc>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    var result = new ToDoItemGrpc
                    {
                        Description = source.Description,
                        Id = resolutionContext.Mapper.Map<ByteString>(source.Id),
                        Name = source.Name,
                        IsCurrent = source.IsCurrent,
                    };

                    result.Parents.AddRange(
                        resolutionContext.Mapper.Map<IEnumerable<ToDoItemParentGrpc>>(source.Parents)
                    );

                    result.Items.AddRange(resolutionContext.Mapper.Map<IEnumerable<ToDoSubItemGrpc>>(source.Items));

                    switch (source)
                    {
                        case ToDoItemGroup toDoItemGroup:
                            result.Group = resolutionContext.Mapper.Map<ToDoItemGroupGrpc>(toDoItemGroup);

                            break;
                        case ToDoItemPeriodicity toDoItemPeriodicity:
                            result.Periodicity =
                                resolutionContext.Mapper.Map<ToDoItemPeriodicityGrpc>(toDoItemPeriodicity);

                            break;
                        case ToDoItemPlanned toDoItemPlanned:
                            result.Planned = resolutionContext.Mapper.Map<ToDoItemPlannedGrpc>(toDoItemPlanned);

                            break;
                        case ToDoItemValue toDoItemValue:
                            result.Value = resolutionContext.Mapper.Map<ToDoItemValueGrpc>(toDoItemValue);

                            break;
                        case ToDoItemPeriodicityOffset doItemPeriodicityOffset:
                            result.PeriodicityOffset =
                                resolutionContext.Mapper.Map<ToDoItemPeriodicityOffsetGrpc>(doItemPeriodicityOffset);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(source));
                    }

                    return result;
                }
            );

        CreateMap<ToDoItemGrpc, IToDoItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.ParametersCase switch
                {
                    ToDoItemGrpc.ParametersOneofCase.Value => new ToDoItemValue(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        resolutionContext.Mapper.Map<IToDoSubItem[]>(source.Items),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.Value.IsCompleted,
                        source.Description,
                        source.IsCurrent,
                        (ToDoItemChildrenType)source.Value.ChildrenType
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Group => new ToDoItemGroup(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Items.Select(x => resolutionContext.Mapper.Map<IToDoSubItem>(x)).ToArray(),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.Description,
                        source.IsCurrent
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Planned => new ToDoItemPlanned(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        resolutionContext.Mapper.Map<IToDoSubItem[]>(source.Items),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Planned.DueDate),
                        source.Planned.IsCompleted,
                        (ToDoItemChildrenType)source.Planned.ChildrenType
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Periodicity => new ToDoItemPeriodicity(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        resolutionContext.Mapper.Map<IToDoSubItem[]>(source.Items),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        resolutionContext.Mapper.Map<IPeriodicity>(source.Periodicity),
                        (ToDoItemChildrenType)source.Periodicity.ChildrenType
                    ),
                    ToDoItemGrpc.ParametersOneofCase.PeriodicityOffset => new ToDoItemPeriodicityOffset(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        resolutionContext.Mapper.Map<IToDoSubItem[]>(source.Items),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsCurrent,
                        (ushort)source.PeriodicityOffset.DaysOffset,
                        (ushort)source.PeriodicityOffset.MonthsOffset,
                        (ushort)source.PeriodicityOffset.WeeksOffset,
                        (ushort)source.PeriodicityOffset.YearsOffset,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.PeriodicityOffset.DueDate),
                        (ToDoItemChildrenType)source.PeriodicityOffset.ChildrenType
                    ),
                    ToDoItemGrpc.ParametersOneofCase.None => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<IToDoSubItem, ToDoSubItemGrpc>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    var result = new ToDoSubItemGrpc
                    {
                        Status = (ToDoItemStatusGrpc)source.Status,
                        Description = source.Description,
                        Id = resolutionContext.Mapper.Map<ByteString>(source.Id),
                        Name = source.Name,
                        OrderIndex = source.OrderIndex,
                        IsCurrent = source.IsCurrent,
                        Active = resolutionContext.Mapper.Map<ActiveToDoItemGrpc>(source.Active),
                        LastCompleted = resolutionContext.Mapper.Map<DateTimeOffsetGrpc>(source.LastCompleted),
                    };

                    switch (source)
                    {
                        case ToDoSubItemGroup toDoSubItemGroup:
                            result.Group = resolutionContext.Mapper.Map<ToDoSubItemGroupGrpc>(toDoSubItemGroup);

                            break;
                        case ToDoSubItemPeriodicity toDoSubItemPeriodicity:
                            result.Periodicity =
                                resolutionContext.Mapper.Map<ToDoSubItemPeriodicityGrpc>(toDoSubItemPeriodicity);

                            break;
                        case ToDoSubItemPlanned toDoSubItemPlanned:
                            result.Planned = resolutionContext.Mapper.Map<ToDoSubItemPlannedGrpc>(toDoSubItemPlanned);

                            break;
                        case ToDoSubItemValue toDoSubItemValue:
                            result.Value = resolutionContext.Mapper.Map<ToDoSubItemValueGrpc>(toDoSubItemValue);

                            break;
                        case ToDoSubItemPeriodicityOffset subItemPeriodicityOffset:
                            result.PeriodicityOffset =
                                resolutionContext.Mapper.Map<ToDoSubItemPeriodicityOffsetGrpc>(
                                    subItemPeriodicityOffset
                                );

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(source));
                    }

                    return result;
                }
            );

        CreateMap<ToDoSubItemGrpc, IToDoSubItem>()
            .ConvertUsing(
                (source, _, resolutionContext) => source.ParametersCase switch
                {
                    ToDoSubItemGrpc.ParametersOneofCase.Value => new ToDoSubItemValue(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Value.IsCompleted,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.Value.CompletedCount,
                        source.Value.SkippedCount,
                        source.Value.FailedCount,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        resolutionContext.Mapper.Map<DateTimeOffset?>(source.LastCompleted)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Group => new ToDoSubItemGroup(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        resolutionContext.Mapper.Map<DateTimeOffset?>(source.LastCompleted)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.None => throw new ArgumentOutOfRangeException(),
                    ToDoSubItemGrpc.ParametersOneofCase.Planned => new ToDoSubItemPlanned(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Planned.DueDate),
                        source.Planned.CompletedCount,
                        source.Planned.SkippedCount,
                        source.Planned.FailedCount,
                        source.Planned.IsCompleted,
                        resolutionContext.Mapper.Map<DateTimeOffset?>(source.LastCompleted)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Periodicity => new ToDoSubItemPeriodicity(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        source.Periodicity.CompletedCount,
                        source.Periodicity.SkippedCount,
                        source.Periodicity.FailedCount,
                        resolutionContext.Mapper.Map<DateTimeOffset?>(source.LastCompleted)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.PeriodicityOffset => new ToDoSubItemPeriodicityOffset(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.PeriodicityOffset.DueDate),
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        source.PeriodicityOffset.CompletedCount,
                        source.PeriodicityOffset.SkippedCount,
                        source.PeriodicityOffset.FailedCount,
                        resolutionContext.Mapper.Map<DateTimeOffset?>(source.LastCompleted)
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                }
            );

        CreateMap<DateTimeOffset?, DateTimeOffsetGrpc>()
            .ConvertUsing(
                x => new DateTimeOffsetGrpc()
                {
                    Date = ToTimestamp(x),
                    Offset = OffsetToDuration(x)
                }
            );

        CreateMap<UpdateToDoItemOrderIndexRequest, UpdateOrderIndexToDoItemOptions>()
            .ConstructUsing(
                (src, res) => new UpdateOrderIndexToDoItemOptions(
                    res.Mapper.Map<Guid>(src.Id),
                    res.Mapper.Map<Guid>(src.TargetId),
                    src.IsAfter
                )
            );
    }

    private DateTimeOffset ToDateTimeOffset(DateTimeOffsetGrpc grpc)
    {
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