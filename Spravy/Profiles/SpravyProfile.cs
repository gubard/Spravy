using System;
using System.Linq;
using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Core.Enums;
using Spravy.Core.Models;
using Spravy.Models;
using Spravy.Protos;
using Spravy.ViewModels;

namespace Spravy.Profiles;

public class SpravyProfile : Profile
{
    public SpravyProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoItemParent, ToDoItemParentNotify>();
        CreateMap<AddRootToDoItemOptions, AddRootToDoItemRequest>();
        CreateMap<ToDoSubItem, ToDoItemNotify>();
        CreateMap<ToDoItemViewModel, ToDoItemNotify>();
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<AddToDoItemOptions, AddToDoItemRequest>();
        CreateMap<UpdateOrderIndexToDoItemOptions, UpdateOrderIndexToDoItemRequest>();
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));

        CreateMap<ToDoItemParentGrpc, ToDoItemParent>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => new Guid(x.Id.ToByteArray())));
        CreateMap<AddRootToDoItemViewModel, AddRootToDoItemOptions>()
            .ConstructUsing(x => new AddRootToDoItemOptions(x.Name));
        CreateMap<ToDoItemNotify, AddToDoItemOptions>()
            .ConstructUsing(x => new AddToDoItemOptions(x.Id, x.Name));
        CreateMap<ByteString, Guid>()
            .ConvertUsing(x => new Guid(x.ToByteArray()));
        CreateMap<ByteString, Guid?>()
            .ConvertUsing(x => x.IsEmpty ? null : new Guid(x.ToByteArray()));
        CreateMap<Guid?, ByteString>()
            .ConvertUsing(x => x.HasValue ? ByteString.CopyFrom(x.Value.ToByteArray()) : ByteString.Empty);

        CreateMap<DateTimeOffset?, DateTimeOffsetGrpc>()
            .ConvertUsing(
                x => new DateTimeOffsetGrpc
                {
                    Date = ToTimestamp(x),
                    Offset = OffsetToDuration(x)
                }
            );
        
        CreateMap<DateTimeOffset, DateTimeOffsetGrpc>()
            .ConvertUsing(
                x => new DateTimeOffsetGrpc
                {
                    Date = ToTimestamp(x),
                    Offset = OffsetToDuration(x)
                }
            );

        CreateMap<ToDoItemGrpc, ToDoItem>()
            .ConstructUsing(
                (src, res) => new ToDoItem(
                    res.Mapper.Map<Guid>(src.Id),
                    src.Name,
                    (TypeOfPeriodicity)src.TypeOfPeriodicity,
                    res.Mapper.Map<DateTimeOffset?>(src.DueDate),
                    src.Items.Select(x => res.Mapper.Map<ToDoSubItem>(x)).ToArray(),
                    src.Parents.Select(x => res.Mapper.Map<ToDoItemParent>(x)).ToArray(),
                    src.IsComplete,
                    src.Description
                )
            );

        CreateMap<ToDoSubItemGrpc, ToDoSubItem>()
            .ConstructUsing(
                (src, res) => new ToDoSubItem(
                    res.Mapper.Map<Guid>(src.Id),
                    src.Name,
                    src.IsComplete,
                    res.Mapper.Map<DateTimeOffset?>(src.DueDate),
                    src.OrderIndex,
                    (ToDoItemStatus)src.Status,
                    src.Description,
                    src.CompletedCount
                )
            );
    }

    private DateTimeOffset? ToDateTimeOffset(DateTimeOffsetGrpc grpc)
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

    private Timestamp? ToTimestamp(DateTimeOffset? date)
    {
        if (!date.HasValue)
        {
            return null;
        }

        return date.Value.Add(date.Value.Offset).ToTimestamp();
    }

    private Duration? OffsetToDuration(DateTimeOffset? date)
    {
        return date?.Offset.ToDuration();
    }

    private Duration? ToDuration(TimeSpan? time)
    {
        return time?.ToDuration();
    }

    private DateTimeOffset? ToDateTimeOffset(Timestamp? timestamp)
    {
        return timestamp?.ToDateTimeOffset();
    }
}