namespace Spravy.Ui.Profiles;

public class SpravyUiProfile : Profile
{
    public SpravyUiProfile()
    {
        CreateMap<CreateUserViewModel, CreateUserOptions>();
        CreateMap<CreateUserViewModel, User>();
        CreateMap<LoginViewModel, User>();
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
                context.Mapper.Map<Option<Uri>>(x.ToDoItemContent.Link), x.DescriptionContent.Description,
                x.DescriptionContent.Type));
    }
}