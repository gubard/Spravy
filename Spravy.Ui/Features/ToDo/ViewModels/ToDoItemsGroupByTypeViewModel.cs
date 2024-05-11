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
    
    public void Clear()
    {
        Values.Clear();
        Groups.Clear();
        Planneds.Clear();
        Periodicitys.Clear();
        PeriodicityOffsets.Clear();
        Circles.Clear();
        Steps.Clear();
        References.Clear();
    }
    
    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Values.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Value));
        Groups.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Group));
        Planneds.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Planned));
        Periodicitys.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Periodicity));
        PeriodicityOffsets.AddItems(items.Where(x => x.Value.Type == ToDoItemType.PeriodicityOffset));
        Circles.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Circle));
        Steps.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Step));
        References.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Reference));
    }
    
    public void ClearExcept(ReadOnlyMemory<Guid> ids)
    {
        Values.ClearExcept(ids);
        Groups.ClearExcept(ids);
        Planneds.ClearExcept(ids);
        Periodicitys.ClearExcept(ids);
        PeriodicityOffsets.ClearExcept(ids);
        Circles.ClearExcept(ids);
        Steps.ClearExcept(ids);
        References.ClearExcept(ids);
    }
    
    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        switch (item.Value.Type)
        {
            case ToDoItemType.Value:
                Values.UpdateItem(item, updateOrder);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Group:
                Values.RemoveItem(item);
                Groups.UpdateItem(item, updateOrder);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Planned:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.UpdateItem(item, updateOrder);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Periodicity:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.UpdateItem(item, updateOrder);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.PeriodicityOffset:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.UpdateItem(item, updateOrder);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Circle:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.UpdateItem(item, updateOrder);
                Steps.RemoveItem(item);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Step:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.UpdateItem(item, updateOrder);
                References.RemoveItem(item);
                
                break;
            case ToDoItemType.Reference:
                Values.RemoveItem(item);
                Groups.RemoveItem(item);
                Planneds.RemoveItem(item);
                Periodicitys.RemoveItem(item);
                PeriodicityOffsets.RemoveItem(item);
                Circles.RemoveItem(item);
                Steps.RemoveItem(item);
                References.UpdateItem(item, updateOrder);
                
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}