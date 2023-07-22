using System;
using AutoMapper;
using Spravy.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Profiles;

public class SpravyUiProfile : Profile
{
    public SpravyUiProfile()
    {
        CreateMap<ToDoItemParent, ToDoItemParentNotify>();
        CreateMap<ToDoSubItemValue, ToDoSubItemValueNotify>();
        CreateMap<ToDoSubItemGroup, ToDoSubItemGroupNotify>();
        CreateMap<ToDoItemValueViewModel, ToDoSubItemValueNotify>();
        CreateMap<ToDoItemGroupViewModel, ToDoSubItemGroupNotify>();

        CreateMap<AddRootToDoItemViewModel, AddRootToDoItemOptions>()
            .ConstructUsing(x => new AddRootToDoItemOptions(x.Name));
        CreateMap<ToDoSubItemValueNotify, AddToDoItemOptions>()
            .ConstructUsing(x => new AddToDoItemOptions(x.Id, x.Name));

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

        CreateMap<ToDoItemViewModel, ToDoSubItemNotify>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    switch (source)
                    {
                        case ToDoItemGroupViewModel toDoItemGroupViewModel:
                            return resolutionContext.Mapper.Map<ToDoSubItemGroupNotify>(toDoItemGroupViewModel);
                        case ToDoItemValueViewModel toDoItemValueViewModel:
                            return resolutionContext.Mapper.Map<ToDoSubItemValueNotify>(toDoItemValueViewModel);
                        default: throw new ArgumentOutOfRangeException(nameof(source));
                    }
                }
            );
    }
}