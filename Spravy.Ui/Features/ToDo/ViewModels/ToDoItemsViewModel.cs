namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    public AvaloniaList<CommandItem> Commands { get; } = new();
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();
    
    [Reactive]
    public TextLocalization? Header { get; set; }
    
    [Reactive]
    public bool IsExpanded { get; set; } = true;
    
    public void Clear()
    {
        Items.Clear();
    }
    
    public void AddItems(IEnumerable<ToDoItemEntityNotify> items)
    {
        Items.AddRange(items);
    }
    
    public void ClearExcept(ReadOnlyMemory<Guid> ids)
    {
        Items.RemoveAll(Items.Where(x => !ids.Span.Contains(x.Id)));
    }
    
    public void UpdateItem(ToDoItemEntityNotify item)
    {
        var indexOf = Items.IndexOf(item);
        var needIndex = GetNeedIndex(item);
        
        if (needIndex == Items.Count)
        {
            Items.Add(item);
        }
        else if (indexOf == -1)
        {
            Items.Insert(GetNeedIndex(item), item);
        }
        else
        {
            Items.Move(indexOf, needIndex);
        }
    }
    
    private int GetNeedIndex(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0)
        {
            return 0;
        }
        
        if (obj.OrderIndex >= Items[^1].OrderIndex)
        {
            return Items.Count;
        }
        
        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Id == Items[i].Id)
            {
                continue;
            }
            
            if (obj.OrderIndex >= Items[i].OrderIndex)
            {
                continue;
            }
            
            return i;
        }
        
        return Items.Count;
    }
    
    private void AddSorted(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0 || obj.OrderIndex >= Items[^1].OrderIndex)
        {
            Items.Add(obj);
            
            return;
        }
        
        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.OrderIndex >= Items[i].OrderIndex)
            {
                continue;
            }
            
            Items.Insert(i, obj);
            
            break;
        }
    }
    
    public void RemoveItem(ToDoItemEntityNotify item)
    {
        Items.Remove(item);
    }
}