using AutoMapper;
using Spravy.Domain.Models;
using Spravy.Db.Models;

namespace Spravy.Service.Profiles;

public class SpravyServiceProfile : Profile
{
    public SpravyServiceProfile()
    {
        CreateMap<AddRootToDoItemOptions, ToDoItemEntity>();
        CreateMap<AddToDoItemOptions, ToDoItemEntity>();
        CreateMap<ToDoItemEntity, ToDoItemParent>()
            .ConstructUsing(x => new ToDoItemParent(x.Id, x.Name));
    }
}