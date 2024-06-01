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
        
        this.WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                Values.IsMulti = x;
                Groups.IsMulti = x;
                Planneds.IsMulti = x;
                Periodicitys.IsMulti = x;
                PeriodicityOffsets.IsMulti = x;
                Circles.IsMulti = x;
                Steps.IsMulti = x;
                References.IsMulti = x;
            });
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
        return Values.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Value).ToArray())
           .IfSuccess(() => Groups.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Group).ToArray()))
           .IfSuccess(() =>
                Planneds.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Planned).ToArray()))
           .IfSuccess(() =>
                Periodicitys.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Periodicity).ToArray()))
           .IfSuccess(() =>
                PeriodicityOffsets.ClearExceptUi(items.ToArray()
                   .Where(x => x.Type == ToDoItemType.PeriodicityOffset)
                   .ToArray()))
           .IfSuccess(() => Circles.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Circle).ToArray()))
           .IfSuccess(() => Steps.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Step).ToArray()))
           .IfSuccess(() =>
                References.ClearExceptUi(items.ToArray().Where(x => x.Type == ToDoItemType.Reference).ToArray()));
    }
    
    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                return Values.UpdateItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Group:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.UpdateItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Planned:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.UpdateItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Periodicity:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.UpdateItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.PeriodicityOffset:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.UpdateItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Circle:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.UpdateItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Step:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.UpdateItemUi(item))
                   .IfSuccess(() => References.RemoveItemUi(item));
            case ToDoItemType.Reference:
                return Values.RemoveItemUi(item)
                   .IfSuccess(() => Groups.RemoveItemUi(item))
                   .IfSuccess(() => Planneds.RemoveItemUi(item))
                   .IfSuccess(() => Periodicitys.RemoveItemUi(item))
                   .IfSuccess(() => PeriodicityOffsets.RemoveItemUi(item))
                   .IfSuccess(() => Circles.RemoveItemUi(item))
                   .IfSuccess(() => Steps.RemoveItemUi(item))
                   .IfSuccess(() => References.UpdateItemUi(item));
            default: return new(new ToDoItemTypeOutOfRangeError(item.Type));
        }
    }
}