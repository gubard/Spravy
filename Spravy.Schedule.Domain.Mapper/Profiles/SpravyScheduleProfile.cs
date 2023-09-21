using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Domain.Mapper.Profiles;

public class SpravyScheduleProfile : Profile
{
    public SpravyScheduleProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<byte[], ByteString>().ConvertUsing(x => ByteString.CopyFrom(x));
        CreateMap<ByteString, byte[]>().ConstructUsing(x => x.ToByteArray());
        CreateMap<Stream, ByteString>().ConvertUsing(x => ByteString.FromStream(x));
        CreateMap<ByteString, Stream>().ConstructUsing(x => new MemoryStream(x.ToByteArray()));
        CreateMap<TimerItem, TimerItemGrpc>();
        CreateMap<TimerItemGrpc, TimerItem>();
        CreateMap<AddTimerParametersGrpc, AddTimerParameters>();
        CreateMap<AddTimerParameters, AddTimerParametersGrpc>();
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToNullableDateTimeOffset(x));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));

        CreateMap<DateTimeOffset, DateTimeOffsetGrpc>()
            .ConvertUsing(
                (source, _, _) => new DateTimeOffsetGrpc
                {
                    Date = ToTimestamp(source),
                    Offset = OffsetToDuration(source),
                }
            );

        CreateMap<AddTimerParameters, AddTimerRequest>()
            .ConvertUsing(
                (source, _, resolutionContext) => new AddTimerRequest
                {
                    Parameters = new AddTimerParametersGrpc
                    {
                        DueDateTime = resolutionContext.Mapper.Map<DateTimeOffsetGrpc>(source.DueDateTime),
                        EventId = resolutionContext.Mapper.Map<ByteString>(source.EventId),
                        Content = resolutionContext.Mapper.Map<ByteString>(source.Content),
                    }
                }
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