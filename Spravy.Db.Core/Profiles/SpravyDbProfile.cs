using AutoMapper;
using Spravy.Db.Models;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Db.Core.Profiles;

public class SpravyDbProfile : Profile
{
    public const string StatusName = "Status";
    public const string ParentsName = "Parents";
    public const string ItemsName = "Items";
    public const string ValueName = "Value";
    public const string ActiveName = "Active";

    public SpravyDbProfile()
    {
        CreateMap<ToDoItemEntity, ActiveToDoItem>()
            .ConvertUsing((source, _, _) => new ActiveToDoItem(source.Id, source.Name));

        CreateMap<ToDoItemEntity, IToDoSubItem>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    switch (source.Type)
                    {
                        case ToDoItemType.Value:
                            return new ToDoSubItemValue(
                                source.Id,
                                source.Name,
                                source.Value.IsComplete,
                                source.Value.DueDate,
                                source.OrderIndex,
                                (ToDoItemStatus)resolutionContext.Items[StatusName],
                                source.Description,
                                source.Statistical.CompletedCount,
                                source.Statistical.SkippedCount,
                                source.Statistical.FailedCount,
                                source.IsCurrent,
                                (ActiveToDoItem?)resolutionContext.Items[ActiveName]
                            );
                        case ToDoItemType.Group:
                            return new ToDoSubItemGroup(
                                source.Id,
                                source.Name,
                                source.OrderIndex,
                                (ToDoItemStatus)resolutionContext.Items[StatusName],
                                source.Description,
                                source.IsCurrent,
                                (ActiveToDoItem?)resolutionContext.Items[ActiveName]
                            );
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            );

        CreateMap<ToDoItemEntity, IToDoItem>()
            .ConvertUsing(
                (source, _, resolutionContext) =>
                {
                    switch (source.Type)
                    {
                        case ToDoItemType.Value:
                            var value = source.Value ?? (ToDoItemValueEntity)resolutionContext.Items[ValueName];

                            return new ToDoItemValue(
                                source.Id,
                                source.Name,
                                value.TypeOfPeriodicity,
                                value.DueDate,
                                (IToDoSubItem[])resolutionContext.Items[ItemsName],
                                (ToDoItemParent[])resolutionContext.Items[ParentsName],
                                value.IsComplete,
                                source.Description,
                                source.IsCurrent
                            );
                        case ToDoItemType.Group:
                            return new ToDoItemGroup(
                                source.Id,
                                source.Name,
                                (IToDoSubItem[])resolutionContext.Items[ItemsName],
                                (ToDoItemParent[])resolutionContext.Items[ParentsName],
                                source.Description,
                                source.IsCurrent
                            );
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            );
    }
}