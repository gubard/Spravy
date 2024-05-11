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
    
    public void ClearExcept(ReadOnlyMemory<ToDoItemEntityNotify> ids)
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
    
    public void UpdateItem(ToDoItemEntityNotify item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                Values.UpdateItem(item);
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
                Groups.UpdateItem(item);
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
                Planneds.UpdateItem(item);
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
                Periodicitys.UpdateItem(item);
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
                PeriodicityOffsets.UpdateItem(item);
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
                Circles.UpdateItem(item);
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
                Steps.UpdateItem(item);
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
                References.UpdateItem(item);
                
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}