using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Domain.Enums;
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
        CreateMap<GetReferenceToDoItemSettingsReply, ReferenceToDoItemSettings>();
        CreateMap<ReferenceToDoItemSettings, GetReferenceToDoItemSettingsReply>();
        CreateMap<AnnuallyPeriodicityGrpc, AnnuallyPeriodicity>();
        CreateMap<AnnuallyPeriodicity, AnnuallyPeriodicityGrpc>();
        CreateMap<MonthlyPeriodicityGrpc, MonthlyPeriodicity>();
        CreateMap<MonthlyPeriodicity, MonthlyPeriodicityGrpc>();
        CreateMap<ResetToDoItemOptions, ResetToDoItemRequest>();
        CreateMap<ResetToDoItemRequest, ResetToDoItemOptions>();
        CreateMap<WeeklyPeriodicityGrpc, WeeklyPeriodicity>().ConvertUsing(x => new(x.Days.Select(y => (DayOfWeek)y)));
        CreateMap<DayOfWeek, DayOfWeekGrpc>();
        CreateMap<DayOfWeekGrpc, DayOfWeek>();
        CreateMap<GetMonthlyPeriodicityReply, MonthlyPeriodicity>().ConvertUsing(x => new(x.Days.Select(y => (byte)y)));
        CreateMap<DescriptionType, DescriptionTypeGrpc>();
        CreateMap<DescriptionTypeGrpc, DescriptionType>();
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
        CreateMap<ActiveToDoItem, ActiveToDoItemGrpc>();
        CreateMap<DateOnly, Timestamp>().ConvertUsing(x => Timestamp.FromDateTime(x.ToDateTime(DateTimeKind.Utc)));
        CreateMap<Timestamp, DateOnly>().ConvertUsing(x => x.ToDateTime().ToDateOnly());
        CreateMap<DateOnly, DateTimeOffset>().ConvertUsing(x => x.ToDateTimeOffset());
        CreateMap<DateTimeOffset, DateOnly>().ConvertUsing(x => x.Date.ToDateOnly());
        CreateMap<DailyPeriodicity, DailyPeriodicityGrpc>();
        CreateMap<AddToDoItemRequest, AddToDoItemOptions>();
        CreateMap<AddRootToDoItemRequest, AddRootToDoItemOptions>();
        CreateMap<TimeSpan, Duration>().ConvertUsing(x => TimeSpanToDuration(x));
        CreateMap<Duration, TimeSpan>().ConvertUsing(x => DurationToTimeSpan(x));
        CreateMap<DayOfYearGrpc, DayOfYear>().ConstructUsing(x => new((byte)x.Day, (byte)x.Month));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoItemType, ToDoItemTypeGrpc>();
        CreateMap<ToDoItemTypeGrpc, ToDoItemType>();
        CreateMap<ToDoItemIsCan, ToDoItemIsCanGrpc>();
        CreateMap<ToDoItemIsCanGrpc, ToDoItemIsCan>();
        CreateMap<string?, Option<Uri>>().ConvertUsing((x, _, _) => x.IsNullOrWhiteSpace() ? new(null) : new(new(x)));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new(x.ToByteArray()));
        
        CreateMap<Option<Uri>, string>()
           .ConvertUsing((x, _, _) => x.TryGetValue(out var value) ? value.AbsoluteUri : string.Empty);
        
        CreateMap<ByteString, OptionStruct<Guid>>()
           .ConvertUsing(x => x.IsEmpty ? new(null) : new(new(x.ToByteArray())));
        
        CreateMap<OptionStruct<Guid>, ByteString>()
           .ConvertUsing((x, _, _) =>
                x.TryGetValue(out var value) ? ByteString.CopyFrom(value.ToByteArray()) : ByteString.Empty);
        
        CreateMap<GetAnnuallyPeriodicityReply, AnnuallyPeriodicity>()
           .ConvertUsing(x => new(x.Days.Select(y => new DayOfYear((byte)y.Day, (byte)y.Month))));
        
        CreateMap<OptionStruct<ActiveToDoItem>, ActiveToDoItemGrpc?>()
           .ConvertUsing((x, _, context) => x.IsHasValue ? context.Mapper.Map<ActiveToDoItemGrpc>(x.Value) : null);
        
        CreateMap<OptionString, Option<Uri>>()
           .ConvertUsing((x, _, _) => x.TryGetValue(out var value) ? new(new(value)) : new(null));
        
        CreateMap<Option<Uri>, OptionString>()
           .ConvertUsing((x, _, _) => x.TryGetValue(out var value) ? new(value.AbsoluteUri) : new(null));
        
        CreateMap<ToDoItemToStringRequest, ToDoItemToStringOptions>()
           .ConstructUsing((x, context) => new(context.Mapper.Map<IEnumerable<ToDoItemStatus>>(x.Statuses),
                context.Mapper.Map<Guid>(x.Id)));
        
        CreateMap<MonthlyPeriodicityGrpc, MonthlyPeriodicity>()
           .ConvertUsing((source, _, _) => new(source.Days.ToByteArray()));
        
        CreateMap<ToDoSelectorItemGrpc, ToDoSelectorItem>()
           .ConvertUsing((source, _, resolutionContext) => new(resolutionContext.Mapper.Map<Guid>(source.Id),
                source.Name, resolutionContext.Mapper.Map<ToDoSelectorItem[]>(source.Children)));
        
        CreateMap<AnnuallyPeriodicityGrpc, AnnuallyPeriodicity>()
           .ConvertUsing((source, _, context) => new(context.Mapper.Map<IEnumerable<DayOfYear>>(source.Days)));
        
        CreateMap<GetWeeklyPeriodicityReply, WeeklyPeriodicity>()
           .ConvertUsing(x => new(x.Days.Select(y => (DayOfWeek)y)));
        
        CreateMap<UpdateToDoItemOrderIndexRequest, UpdateOrderIndexToDoItemOptions>()
           .ConstructUsing((src, res) =>
                new(res.Mapper.Map<Guid>(src.Id), res.Mapper.Map<Guid>(src.TargetId), src.IsAfter));
        
        CreateMap<WeeklyPeriodicity, WeeklyPeriodicityGrpc>()
           .ConvertUsing((x, _, _) =>
            {
                var result = new WeeklyPeriodicityGrpc();
                result.Days.AddRange(x.Days.Select(y => (DayOfWeekGrpc)y));
                
                return result;
            });
        
        CreateMap<WeeklyPeriodicity, GetWeeklyPeriodicityReply>()
           .ConvertUsing((x, _, _) =>
            {
                var result = new GetWeeklyPeriodicityReply();
                result.Days.AddRange(x.Days.Select(y => (DayOfWeekGrpc)y));
                
                return result;
            });
        
        CreateMap<MonthlyPeriodicity, GetMonthlyPeriodicityReply>()
           .ConvertUsing((x, _, _) =>
            {
                var result = new GetMonthlyPeriodicityReply();
                result.Days.AddRange(x.Days.Select(y => (uint)y));
                
                return result;
            });
        
        CreateMap<AnnuallyPeriodicity, GetAnnuallyPeriodicityReply>()
           .ConvertUsing((x, _, _) =>
            {
                var result = new GetAnnuallyPeriodicityReply();
                
                result.Days.AddRange(x.Days.Select(y => new DayOfYearGrpc
                {
                    Day = y.Day,
                    Month = y.Month,
                }));
                
                return result;
            });
        
        CreateMap<ToDoSelectorItem, ToDoSelectorItemGrpc>()
           .ConvertUsing((x, _, context) =>
            {
                var result = new ToDoSelectorItemGrpc
                {
                    Name = x.Name,
                    Id = context.Mapper.Map<ByteString>(x.Id),
                };
                
                result.Children.AddRange(x.Children.ToArray().Select(y => context.Mapper.Map<ToDoSelectorItemGrpc>(y)));
                
                return result;
            });
        
        CreateMap<ToDoItemToStringOptions, ToDoItemToStringRequest>()
           .ConvertUsing((x, _, context) =>
            {
                var request = new ToDoItemToStringRequest
                {
                    Id = context.Mapper.Map<ByteString>(x.Id),
                };
                
                request.Statuses.AddRange(context.Mapper.Map<IEnumerable<ToDoItemStatusGrpc>>(x.Statuses.ToArray()));
                
                return request;
            });
        
        CreateMap<DayOfYear, DayOfYearGrpc>()
           .ConvertUsing((source, _, _) => new()
            {
                Day = source.Day,
                Month = source.Month,
            });
        
        CreateMap<MonthlyPeriodicity, MonthlyPeriodicityGrpc>()
           .ConvertUsing((source, _, _) => new()
            {
                Days = ByteString.CopyFrom(source.Days.ToArray()),
            });
        
        CreateMap<AnnuallyPeriodicity, AnnuallyPeriodicityGrpc>()
           .ConvertUsing((source, _, resolutionContext) =>
            {
                var result = new AnnuallyPeriodicityGrpc();
                result.Days.AddRange(source.Days.Select(x => resolutionContext.Mapper.Map<DayOfYearGrpc>(x)));
                
                return result;
            });
        
        CreateMap<ToDoSubItemPeriodicityGrpc, IPeriodicity>()
           .ConvertUsing((source, _, resolutionContext) => source.PeriodicityCase switch
            {
                ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.None => throw new ArgumentOutOfRangeException(),
                ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Daily => new DailyPeriodicity(),
                ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Weekly => new WeeklyPeriodicity(
                    source.Weekly.Days.Select(x => (DayOfWeek)x)),
                ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Monthly => new MonthlyPeriodicity(
                    source.Monthly.ToByteArray()),
                ToDoSubItemPeriodicityGrpc.PeriodicityOneofCase.Annually => new AnnuallyPeriodicity(
                    resolutionContext.Mapper.Map<IEnumerable<DayOfYear>>(source.Annually.Days)),
                _ => throw new ArgumentOutOfRangeException(),
            });
        
        CreateMap<ToDoItemPeriodicityGrpc, IPeriodicity>()
           .ConvertUsing((source, _, resolutionContext) => source.PeriodicityCase switch
            {
                ToDoItemPeriodicityGrpc.PeriodicityOneofCase.None => throw new ArgumentOutOfRangeException(),
                ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Daily => new DailyPeriodicity(),
                ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Weekly => new WeeklyPeriodicity(
                    source.Weekly.Days.Select(x => (DayOfWeek)x)),
                ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Monthly => new MonthlyPeriodicity(
                    source.Monthly.Days.ToArray()),
                ToDoItemPeriodicityGrpc.PeriodicityOneofCase.Annually => new AnnuallyPeriodicity(
                    resolutionContext.Mapper.Map<IEnumerable<DayOfYear>>(source.Annually.Days)),
                _ => throw new ArgumentOutOfRangeException(),
            });
        
        CreateMap<ActiveToDoItemGrpc, OptionStruct<ActiveToDoItem>>()
           .ConvertUsing((source, _, resolutionContext) =>
            {
                if (source is null)
                {
                    return new(null);
                }
                
                return new(new(resolutionContext.Mapper.Map<Guid>(source.Id), source.Name));
            });
        
        CreateMap<DateTimeOffset, DateTimeOffsetGrpc>()
           .ConvertUsing((source, _, _) => new()
            {
                Date = ToTimestamp(source),
                Offset = OffsetToDuration(source),
            });
        
        CreateMap<DateTimeOffset?, DateTimeOffsetGrpc>()
           .ConvertUsing(x => new()
            {
                Date = ToTimestamp(x),
                Offset = OffsetToDuration(x),
            });
    }
    
    private DateTimeOffset ToDateTimeOffset(DateTimeOffsetGrpc grpc)
    {
        var date = grpc.Date.ToDateTimeOffset();
        
        return new(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, grpc.Offset.ToTimeSpan());
    }
    
    private Duration? OffsetToDuration(DateTimeOffset? date)
    {
        return date?.Offset.ToDuration();
    }
    
    private Duration TimeSpanToDuration(TimeSpan span)
    {
        return Duration.FromTimeSpan(span);
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