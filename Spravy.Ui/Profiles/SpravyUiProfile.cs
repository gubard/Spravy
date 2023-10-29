using System;
using System.IO;
using AutoMapper;
using Spravy.Authentication.Domain.Models;
using Spravy.EventBus.Protos;
using Spravy.Schedule.Domain.Models;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using Google.Protobuf;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

namespace Spravy.Ui.Profiles;

public class SpravyUiProfile : Profile
{
    public SpravyUiProfile()
    {
        CreateMap<ToDoItemParent, ToDoItemParentNotify>();
        CreateMap<ToDoItemNotify, ToDoShortItem>();
        CreateMap<ToDoShortItem, ToDoItemNotify>();
        CreateMap<ToDoSubItemValue, ToDoSubItemValueNotify>();
        CreateMap<ToDoSubItemGroup, ToDoSubItemGroupNotify>();
        CreateMap<ToDoSubItemPeriodicity, ToDoSubItemPeriodicityNotify>();
        CreateMap<ToDoSubItemPeriodicityOffset, ToDoSubItemPeriodicityOffsetNotify>();
        CreateMap<ToDoSubItemPlanned, ToDoSubItemPlannedNotify>();
        CreateMap<ToDoItemValueViewModel, ToDoSubItemValueNotify>();
        CreateMap<ToDoItemPeriodicityOffsetViewModel, ToDoSubItemPeriodicityOffsetNotify>();
        CreateMap<ToDoItemGroupViewModel, ToDoSubItemGroupNotify>();
        CreateMap<ToDoItemPlannedViewModel, ToDoSubItemPlannedNotify>();
        CreateMap<ToDoItemPeriodicityViewModel, ToDoSubItemPeriodicityNotify>();
        CreateMap<ToDoSelectorItem, ToDoSelectorItemNotify>();
        CreateMap<CreateUserViewModel, CreateUserOptions>();
        CreateMap<CreateUserViewModel, User>();
        CreateMap<LoginViewModel, User>();
        CreateMap<TimerItem, TimerItemToDoItemPinnedNotify>()
            .ConvertUsing(
                (item, _, _) => new TimerItemToDoItemPinnedNotify
                {
                    DueDateTime = item.DueDateTime,
                    Id = item.Id,
                    IsPinned = ChangeToDoItemIsPinnedEvent.Parser.ParseFrom(item.Content).IsPinned
                }
            );

        CreateMap<TimerItem, TimerItemNotify>()
            .ConvertUsing((item, _, context) => context.Mapper.Map<TimerItemToDoItemPinnedNotify>(item));
        CreateMap<AddRootToDoItemViewModel, AddRootToDoItemOptions>()
            .ConstructUsing(x => new AddRootToDoItemOptions(x.Name));
        CreateMap<ToDoSubItemValueNotify, AddToDoItemOptions>()
            .ConstructUsing(x => new AddToDoItemOptions(x.Id, x.Name));

        CreateMap<AddTimerViewModel, AddTimerParameters>()
            .ConvertUsing(
                (source, _, context) =>
                {
                    var changeToDoItemIsPinnedEvent = new ChangeToDoItemIsPinnedEvent
                    {
                        IsPinned = source.IsPinned,
                        ToDoItemId = context.Mapper.Map<ByteString>(source.Item.ThrowIfNull().Id),
                    };

                    using var stream = new MemoryStream();
                    changeToDoItemIsPinnedEvent.WriteTo(stream);

                    return new AddTimerParameters(
                        source.DueDateTime,
                        source.EventId,
                        stream.ToByteArray()
                    );
                }
            );

        CreateMap<ActiveToDoItem?, ActiveToDoItemNotify?>()
            .ConvertUsing(
                (source, _, resolutionContext) => source is null
                    ? null
                    : new ActiveToDoItemNotify
                    {
                        Id = resolutionContext.Mapper.Map<Guid>(source.Value.Id),
                        Name = source.Value.Name
                    }
            );

        CreateMap<ToDoSubItemValue, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) => resolutionContext.Mapper.Map<ToDoSubItemValueNotify>(source)
            );

        CreateMap<ToDoSubItemGroup, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) => resolutionContext.Mapper.Map<ToDoSubItemGroupNotify>(source)
            );

        CreateMap<ToDoSubItemPlanned, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) => resolutionContext.Mapper.Map<ToDoSubItemPlannedNotify>(source)
            );

        CreateMap<ToDoSubItemPeriodicity, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) => resolutionContext.Mapper.Map<ToDoSubItemPeriodicityNotify>(source)
            );

        CreateMap<ToDoSubItemPeriodicityOffset, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                    resolutionContext.Mapper.Map<ToDoSubItemPeriodicityOffsetNotify>(source)
            );

        CreateMap<ToDoItemViewModel, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) => source switch
                {
                    ToDoItemGroupViewModel toDoItemGroupViewModel => resolutionContext.Mapper
                        .Map<ToDoSubItemGroupNotify>(toDoItemGroupViewModel),
                    ToDoItemPeriodicityViewModel toDoItemPeriodicityViewModel => resolutionContext.Mapper
                        .Map<ToDoSubItemPeriodicityNotify>(toDoItemPeriodicityViewModel),
                    ToDoItemPlannedViewModel toDoItemPlannedViewModel => resolutionContext.Mapper
                        .Map<ToDoSubItemPlannedNotify>(toDoItemPlannedViewModel),
                    ToDoItemValueViewModel toDoItemValueViewModel => resolutionContext.Mapper
                        .Map<ToDoSubItemValueNotify>(toDoItemValueViewModel),
                    ToDoItemPeriodicityOffsetViewModel toDoItemPeriodicityOffsetViewModel => resolutionContext.Mapper
                        .Map<ToDoSubItemPeriodicityOffsetNotify>(toDoItemPeriodicityOffsetViewModel),
                    _ => throw new ArgumentOutOfRangeException(nameof(source))
                }
            );
    }
}