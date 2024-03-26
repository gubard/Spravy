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
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.Ui.Features.PasswordGenerator.Models;
using Spravy.Ui.Features.PasswordGenerator.ViewModels;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Profiles;

public class SpravyUiProfile : Profile
{
    public SpravyUiProfile()
    {
        CreateMap<ToDoItem, ToDoItemNotify>();
        CreateMap<ToDoShortItemNotify, ToDoShortItem>();
        CreateMap<ToDoShortItem, ToDoShortItemNotify>();
        CreateMap<ToDoSelectorItem, ToDoSelectorItemNotify>();
        CreateMap<ToDoItem, ToDoSelectorItemNotify>();
        CreateMap<CreateUserViewModel, CreateUserOptions>();
        CreateMap<CreateUserViewModel, User>();
        CreateMap<LoginViewModel, User>();
        CreateMap<ToDoShortItem, ToDoItemParentNotify>();
        CreateMap<ToDoItemViewModel, ToDoItemNotify>();
        CreateMap<PasswordItem, PasswordItemNotify>();
        CreateMap<AddPasswordItemViewModel, AddPasswordOptions>();

        CreateMap<TimerItem, TimerItemNotify>()
            .ConvertUsing((item, _, context) => context.Mapper.Map<TimerItemToDoItemFavoriteNotify>(item));

        CreateMap<TimerItem, TimerItemToDoItemFavoriteNotify>()
            .ConvertUsing(
                (item, _, _) => new()
                {
                    DueDateTime = item.DueDateTime,
                    Id = item.Id,
                    IsFavorite = ChangeToDoItemIsFavoriteEvent.Parser.ParseFrom(item.Content).IsFavorite,
                }
            );

        CreateMap<AddRootToDoItemViewModel, AddRootToDoItemOptions>()
            .ConvertUsing(
                (x, _, context) => new(
                    x.ToDoItemContent.Name,
                    x.ToDoItemContent.Type,
                    context.Mapper.Map<Uri?>(x.ToDoItemContent.Link),
                    x.DescriptionContent.Description,
                    x.DescriptionContent.Type
                )
            );

        CreateMap<AddTimerViewModel, AddTimerParameters>()
            .ConvertUsing(
                (source, _, context) =>
                {
                    var changeToDoItemIsFavoriteEvent = new ChangeToDoItemIsFavoriteEvent
                    {
                        IsFavorite = source.IsFavorite,
                        ToDoItemId = context.Mapper.Map<ByteString>(source.ShortItem.ThrowIfNull().Id),
                    };

                    using var stream = new MemoryStream();
                    changeToDoItemIsFavoriteEvent.WriteTo(stream);

                    return new(
                        source.DueDateTime,
                        source.EventId,
                        stream.ToByteArray()
                    );
                }
            );

        CreateMap<ActiveToDoItem?, ActiveToDoItemNotify?>()
            .ConvertUsing(
                (source, _, context) => source is null
                    ? null
                    : new ActiveToDoItemNotify
                    {
                        Id = context.Mapper.Map<Guid>(source.Value.Id),
                        Name = source.Value.Name
                    }
            );

        CreateMap<ActiveToDoItem, ActiveToDoItemNotify?>()
            .ConvertUsing(
                (source, _, context) => new ActiveToDoItemNotify
                {
                    Id = context.Mapper.Map<Guid>(source.Id),
                    Name = source.Name
                }
            );
    }
}