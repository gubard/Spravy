using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Protos;

namespace Spravy.Domain.Core.Profiles;

public class SpravyProfile : Profile
{
    public SpravyProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ToDoItemParent, ToDoItemParentGrpc>();
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset?>().ConvertUsing(x => ToNullableDateTimeOffset(x));
        CreateMap<DateTimeOffsetGrpc, DateTimeOffset>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<DateTimeOffset?, Timestamp?>().ConvertUsing(x => ToTimestamp(x));
        CreateMap<Timestamp?, DateTimeOffset?>().ConvertUsing(x => ToDateTimeOffset(x));
        CreateMap<ToDoItemParentGrpc, ToDoItemParent>();
        CreateMap<AddRootToDoItemOptions, AddRootToDoItemRequest>();
        CreateMap<ToDoSubItemValue, ToDoSubItemValueGrpc>();
        CreateMap<ToDoItemGroup, ToDoItemGroupGrpc>();
        CreateMap<ToDoItemValue, ToDoItemValueGrpc>();
        CreateMap<ToDoSubItemGroup, ToDoSubItemGroupGrpc>();
        CreateMap<AddToDoItemOptions, AddToDoItemRequest>();
        CreateMap<UpdateOrderIndexToDoItemOptions, UpdateOrderIndexToDoItemRequest>();
        CreateMap<ActiveToDoItem?, ActiveToDoItemGrpc>();
        CreateMap<ActiveToDoItem, ActiveToDoItemGrpc>();
        CreateMap<ToDoItemPlanned, ToDoItemPlannedGrpc>();
        CreateMap<ToDoItemPeriodicity, ToDoItemPeriodicityGrpc>();
        CreateMap<ToDoSubItemPlanned, ToDoSubItemPlannedGrpc>();
        CreateMap<ToDoSubItemPeriodicity, ToDoSubItemPeriodicityGrpc>();


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
                        source.IsCurrent
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
                        source.Planned.IsCompleted
                    ),
                    ToDoItemGrpc.ParametersOneofCase.Periodicity => new ToDoItemPeriodicity(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.Description,
                        resolutionContext.Mapper.Map<IToDoSubItem[]>(source.Items),
                        resolutionContext.Mapper.Map<ToDoItemParent[]>(source.Parents),
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        (TypeOfPeriodicity)source.Periodicity.TypeOfPeriodicity
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
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active)
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Group => new ToDoSubItemGroup(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active)
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
                        source.Planned.IsCompleted
                    ),
                    ToDoSubItemGrpc.ParametersOneofCase.Periodicity => new ToDoSubItemPeriodicity(
                        resolutionContext.Mapper.Map<Guid>(source.Id),
                        source.Name,
                        source.OrderIndex,
                        (ToDoItemStatus)source.Status,
                        source.Description,
                        source.IsCurrent,
                        resolutionContext.Mapper.Map<DateTimeOffset>(source.Periodicity.DueDate),
                        (TypeOfPeriodicity)source.Periodicity.TypeOfPeriodicity,
                        resolutionContext.Mapper.Map<ActiveToDoItem?>(source.Active),
                        source.Periodicity.CompletedCount,
                        source.Periodicity.SkippedCount,
                        source.Periodicity.FailedCount
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

        CreateMap<UpdateOrderIndexToDoItemRequest, UpdateOrderIndexToDoItemOptions>()
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