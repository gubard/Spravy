using Spravy.ToDo.Domain.Errors;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByTypeViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel circles;
    private readonly ToDoItemsViewModel groups;
    private readonly ToDoItemsViewModel periodicityOffsets;
    private readonly ToDoItemsViewModel periodicitys;
    private readonly ToDoItemsViewModel planneds;
    private readonly ToDoItemsViewModel steps;
    private readonly ToDoItemsViewModel values;
    private readonly ToDoItemsViewModel references;
    
    [Inject]
    public required ToDoItemsViewModel Values
    {
        get => values;
        [MemberNotNull(nameof(values))]
        init
        {
            values = value;
            values.Header = new("ToDoItemsGroupByTypeView.Values");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Groups
    {
        get => groups;
        [MemberNotNull(nameof(groups))]
        init
        {
            groups = value;
            groups.Header = new("ToDoItemsGroupByTypeView.Groups");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Planneds
    {
        get => planneds;
        [MemberNotNull(nameof(planneds))]
        init
        {
            planneds = value;
            planneds.Header = new("ToDoItemsGroupByTypeView.Planneds");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Periodicitys
    {
        get => periodicitys;
        [MemberNotNull(nameof(periodicitys))]
        init
        {
            periodicitys = value;
            periodicitys.Header = new("ToDoItemsGroupByTypeView.Periodicitys");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel PeriodicityOffsets
    {
        get => periodicityOffsets;
        [MemberNotNull(nameof(periodicityOffsets))]
        init
        {
            periodicityOffsets = value;
            periodicityOffsets.Header = new("ToDoItemsGroupByTypeView.PeriodicityOffsets");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Circles
    {
        get => circles;
        [MemberNotNull(nameof(circles))]
        init
        {
            circles = value;
            circles.Header = new("ToDoItemsGroupByTypeView.Circles");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Steps
    {
        get => steps;
        [MemberNotNull(nameof(steps))]
        init
        {
            steps = value;
            steps.Header = new("ToDoItemsGroupByTypeView.Steps");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel References
    {
        get => references;
        [MemberNotNull(nameof(references))]
        init
        {
            references = value;
            references.Header = new("ToDoItemsGroupByTypeView.Reference");
        }
    }
    
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