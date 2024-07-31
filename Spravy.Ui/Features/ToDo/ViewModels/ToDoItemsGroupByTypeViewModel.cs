namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByTypeViewModel : ViewModelBase
{
    public ToDoItemsGroupByTypeViewModel(
        ToDoItemsViewModel circles,
        ToDoItemsViewModel groups,
        ToDoItemsViewModel periodicityOffsets,
        ToDoItemsViewModel periodicitys,
        ToDoItemsViewModel planneds,
        ToDoItemsViewModel steps,
        ToDoItemsViewModel values,
        ToDoItemsViewModel references
    )
    {
        circles.Header = new("ToDoItemsGroupByTypeView.Circles");
        Circles = circles;
        groups.Header = new("ToDoItemsGroupByTypeView.Groups");
        Groups = groups;
        periodicityOffsets.Header = new("ToDoItemsGroupByTypeView.PeriodicityOffsets");
        PeriodicityOffsets = periodicityOffsets;
        periodicitys.Header = new("ToDoItemsGroupByTypeView.Periodicitys");
        Periodicitys = periodicitys;
        planneds.Header = new("ToDoItemsGroupByTypeView.Planneds");
        Planneds = planneds;
        steps.Header = new("ToDoItemsGroupByTypeView.Steps");
        Steps = steps;
        values.Header = new("ToDoItemsGroupByTypeView.Values");
        Values = values;
        references.Header = new("ToDoItemsGroupByTypeView.Reference");
        References = references;
    }

    public ToDoItemsViewModel Values { get; }
    public ToDoItemsViewModel Groups { get; }
    public ToDoItemsViewModel Planneds { get; }
    public ToDoItemsViewModel Periodicitys { get; }
    public ToDoItemsViewModel PeriodicityOffsets { get; }
    public ToDoItemsViewModel Circles { get; }
    public ToDoItemsViewModel Steps { get; }
    public ToDoItemsViewModel References { get; }

    [Reactive]
    public bool IsMulti { get; set; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Values
            .ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Value))
            .IfSuccess(() => Groups.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Group)))
            .IfSuccess(
                () => Planneds.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Planned))
            )
            .IfSuccess(
                () =>
                    Periodicitys.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Periodicity))
            )
            .IfSuccess(
                () =>
                    PeriodicityOffsets.ClearExceptUi(
                        items.Where(x => x.Type == ToDoItemType.PeriodicityOffset)
                    )
            )
            .IfSuccess(() => Circles.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Circle)))
            .IfSuccess(() => Steps.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Step)))
            .IfSuccess(
                () => References.ClearExceptUi(items.Where(x => x.Type == ToDoItemType.Reference))
            );
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                return Values
                    .UpdateItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Group:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.UpdateItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Planned:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.UpdateItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Periodicity:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.UpdateItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.PeriodicityOffset:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.UpdateItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Circle:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.UpdateItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Step:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.UpdateItemUi(item))
                    .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Reference:
                return Values
                    .RemoveItemUi(item)
                    .IfSuccess(() => Groups.RemoveItemUi(item))
                    .IfSuccess(() => Planneds.RemoveItemUi(item))
                    .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                    .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                    .IfSuccess(() => Circles.RemoveItemUi(item))
                    .IfSuccess(() => Steps.RemoveItemUi(item))
                    .IfSuccess(() => References.UpdateItemUi(item));
            default:
                return new(new ToDoItemTypeOutOfRangeError(item.Type));
        }
    }
}
