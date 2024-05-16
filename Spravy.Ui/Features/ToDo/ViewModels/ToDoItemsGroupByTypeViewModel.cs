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
    
    public void UpdateItemUi(ToDoItemEntityNotify item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                Values.UpdateItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Group:
                Values.RemoveItemUi(item);
                Groups.UpdateItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Planned:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.UpdateItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Periodicity:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.UpdateItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.PeriodicityOffset:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.UpdateItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Circle:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.UpdateItemUi(item);
                Steps.RemoveItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Step:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.UpdateItemUi(item);
                References.RemoveItemUi(item);
                
                break;
            case ToDoItemType.Reference:
                Values.RemoveItemUi(item);
                Groups.RemoveItemUi(item);
                Planneds.RemoveItemUi(item);
                Periodicitys.RemoveItemUi(item);
                PeriodicityOffsets.RemoveItemUi(item);
                Circles.RemoveItemUi(item);
                Steps.RemoveItemUi(item);
                References.UpdateItemUi(item);
                
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}