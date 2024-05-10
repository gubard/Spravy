using Spravy.Ui.Features.Timer.ViewModels;

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
        CreateMap<AddPasswordItemViewModel, AddPasswordOptions>();
        CreateMap<ResetToDoItemViewModel, ResetToDoItemOptions>();

        CreateMap<TimerItem, TimerItemNotify>()
           .ConvertUsing((item, _, context) => context.Mapper.Map<TimerItemToDoItemFavoriteNotify>(item));

        CreateMap<TimerItem, TimerItemToDoItemFavoriteNotify>()
           .ConvertUsing((item, _, _) => new()
            {
                DueDateTime = item.DueDateTime,
                Id = item.Id,
                IsFavorite = ChangeToDoItemIsFavoriteEvent.Parser.ParseFrom(item.Content).IsFavorite,
            });

        CreateMap<AddRootToDoItemViewModel, AddRootToDoItemOptions>()
           .ConvertUsing((x, _, context) => new(x.ToDoItemContent.Name, x.ToDoItemContent.Type,
                context.Mapper.Map<Uri?>(x.ToDoItemContent.Link), x.DescriptionContent.Description,
                x.DescriptionContent.Type));

        CreateMap<AddTimerViewModel, AddTimerParameters>()
           .ConvertUsing((source, _, context) =>
            {
                var changeToDoItemIsFavoriteEvent = new ChangeToDoItemIsFavoriteEvent
                {
                    IsFavorite = source.IsFavorite,
                    ToDoItemId = context.Mapper.Map<ByteString>(source.ShortItem.ThrowIfNull().Id),
                };

                using var stream = new MemoryStream();
                changeToDoItemIsFavoriteEvent.WriteTo(stream);

                return new(source.DueDateTime, source.EventId, stream.ToByteArray());
            });

        CreateMap<ActiveToDoItem?, ActiveToDoItemNotify?>()
           .ConvertUsing((source, _, context) => source is null
                ? null
                : new ActiveToDoItemNotify
                {
                    Id = context.Mapper.Map<Guid>(source.Value.Id),
                    Name = source.Value.Name,
                });

        CreateMap<ActiveToDoItem, ActiveToDoItemNotify?>()
           .ConvertUsing((source, _, context) => new()
            {
                Id = context.Mapper.Map<Guid>(source.Id),
                Name = source.Name,
            });
    }
}