namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    public AvaloniaList<CommandItem> Commands { get; } = new();
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();
    
    [Reactive]
    public TextLocalization? Header { get; set; }
    
    [Reactive]
    public bool IsExpanded { get; set; } = true;
    
    public void ClearExcept(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        for (var index = 0; index < Items.Count; index++)
        {
            var item = Items[index];
            
            if (!items.Span.Contains(item))
            {
                Items.Remove(item);
            }
        }
        
        for (var index = 0; index < items.Span.Length; index++)
        {
            UpdateItem(items.Span[index]);
        }
    }
    
    public void UpdateItem(ToDoItemEntityNotify item)
    {
        var indexOf = IndexOf(item);
        var needIndex = GetNeedIndex(item);
        
        if (indexOf == needIndex)
        {
            return;
        }
        
        if (indexOf == -1)
        {
            if (needIndex == Items.Count)
            {
                Items.Add(item);
            }
            else
            {
                Items.Insert(needIndex, item);
            }
        }
        else
        {
            if (needIndex == Items.Count)
            {
                Items.Move(indexOf, needIndex - 1);
            }
            else
            {
                Items.Move(indexOf, needIndex);
            }
        }
    }
    
    private int IndexOf(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0)
        {
            return -1;
        }
        
        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Id == Items[i].Id)
            {
                return i;
            }
        }
        
        return -1;
    }
    
    private int GetNeedIndex(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0)
        {
            return 0;
        }
        
        if (obj.OrderIndex > Items[^1].OrderIndex)
        {
            return Items.Count;
        }
        
        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Id == Items[i].Id)
            {
                return i;
            }
            
            if (obj.OrderIndex == Items[i].OrderIndex)
            {
                return i;
            }
            
            if (obj.OrderIndex > Items[i].OrderIndex)
            {
                continue;
            }
            
            return i;
        }
        
        return Items.Count;
    }
    
    public void RemoveItem(ToDoItemEntityNotify item)
    {
        Items.Remove(item);
    }
}