using AutoMapper;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Db.Extensions;
using Spravy.ToDo.Db.Models;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Mapper.Profiles;

public class SpravyToDoDbProfile : Profile
{
    public const string StatusName = "Status";
    public const string ParentsName = "Parents";
    public const string ItemsName = "Items";
    public const string ActiveName = "Active";
    public const string ParametersName = "Parameters";

    public SpravyToDoDbProfile()
    {
        CreateMap<ToDoItemEntity, ToDoItem>()
            .ConvertUsing(
                (entity, _, context) =>
                {
                    var parameters = (ToDoItemParameters)context.Items[ParametersName];

                    return new ToDoItem(
                        entity.Id,
                        entity.Name,
                        entity.IsFavorite,
                        entity.Type,
                        entity.Description,
                        context.Mapper.Map<Uri?>(entity.Link),
                        entity.OrderIndex,
                        parameters.Status,
                        parameters.ActiveItem,
                        parameters.IsCan,
                        entity.ParentId,
                        entity.DescriptionType
                    );
                }
            );

        CreateMap<ToDoItemEntity, PlannedToDoItemSettings>();
        CreateMap<ToDoItemEntity, ValueToDoItemSettings>();
        CreateMap<ToDoItemEntity, PeriodicityToDoItemSettings>();
        CreateMap<ToDoItemEntity, DailyPeriodicity>();
        CreateMap<ToDoItemEntity, PeriodicityOffsetToDoItemSettings>();
        CreateMap<ToDoItemEntity, ToDoShortItem>();
        CreateMap<ToDoItemEntity, MonthlyPeriodicity>().ConstructUsing(x => new(x.GetDaysOfMonth()));
        CreateMap<ToDoItemEntity, WeeklyPeriodicity>().ConstructUsing(x => new(x.GetDaysOfWeek()));
        CreateMap<AddRootToDoItemOptions, ToDoItemEntity>();
        CreateMap<AddToDoItemOptions, ToDoItemEntity>();
        CreateMap<string?, Uri?>().ConvertUsing(str => str.IsNullOrWhiteSpace() ? null : new Uri(str));
        CreateMap<Uri?, string?>().ConvertUsing(uri => uri == null ? string.Empty : uri.AbsoluteUri);

        CreateMap<ToDoItemEntity, AnnuallyPeriodicity>()
            .ConstructUsing(x => new(x.GetDaysOfYear()));

        CreateMap<ToDoItemEntity, ActiveToDoItem>()
            .ConvertUsing((source, _, _) => new(source.Id, source.Name));
    }
}