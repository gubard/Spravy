using AutoMapper;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Service.Profiles;

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