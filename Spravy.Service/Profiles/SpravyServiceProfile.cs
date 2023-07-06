using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Core.Models;
using Spravy.Db.Models;
using Spravy.Protos;

namespace Spravy.Service.Profiles;

public class SpravyServiceProfile : Profile
{
    public SpravyServiceProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoItemParent, ToDoItemParentGrpc>();
        CreateMap<ToDoSubItem, ToDoSubItemGrpc>();
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<ToDoItemParentGrpc, ToDoItemParent>();
        CreateMap<ToDoSubItemGrpc, ToDoSubItem>();
        CreateMap<ToDoItem, ToDoItemGrpc>();
        CreateMap<AddRootToDoItemOptions, ToDoItemEntity>();
        CreateMap<AddToDoItemOptions, ToDoItemEntity>();

        CreateMap<Guid?, ByteString>()
            .ConvertUsing(x => x.HasValue ? ByteString.CopyFrom(x.Value.ToByteArray()) : ByteString.Empty);
        CreateMap<ToDoItemEntity, ToDoItemParent>()
            .ConstructUsing(x => new ToDoItemParent(x.Id, x.Name));
        CreateMap<AddToDoItemRequest, AddToDoItemOptions>()
            .ConstructUsing(x => new AddToDoItemOptions(new Guid(x.ParentId.ToByteArray()), x.Name));
        CreateMap<AddRootToDoItemRequest, AddRootToDoItemOptions>()
            .ConstructUsing(x => new AddRootToDoItemOptions(x.Name));
        CreateMap<ByteString, Guid>()
            .ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<ByteString, Guid?>()
            .ConvertUsing(x => x.IsEmpty ? null : new Guid(x.ToByteArray()));

        CreateMap<DateTimeOffset?, DateTimeOffsetGrpc>()
            .ConvertUsing(
                x => new DateTimeOffsetGrpc()
                {
                    Date = ToTimestamp(x),
                    Offset = OffsetToDuration(x)
                }
            );

        CreateMap<UpdateOrderIndexToDoItemRequest, UpdateOrderIndexToDoItemOptions>()
            .ConstructUsing(
                (src, res) => new UpdateOrderIndexToDoItemOptions(
                    res.Mapper.Map<Guid>(src.Id),
                    res.Mapper.Map<Guid>(src.TargetId),
                    src.IsAfter
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

    private Duration? OffsetToDuration(DateTimeOffset? date)
    {
        return date?.Offset.ToDuration();
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