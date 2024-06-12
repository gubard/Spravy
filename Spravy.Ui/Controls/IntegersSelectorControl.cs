using System.Collections.Specialized;

namespace Spravy.Ui.Controls;

public class IntegersSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<int>?> SelectedIntegersProperty =
        AvaloniaProperty.Register<IntegersSelectorControl, IEnumerable<int>?>(nameof(SelectedIntegers));
    
    public static readonly StyledProperty<int> MinProperty =
        AvaloniaProperty.Register<IntegersSelectorControl, int>(nameof(Min));
    
    public static readonly StyledProperty<int> MaxProperty =
        AvaloniaProperty.Register<IntegersSelectorControl, int>(nameof(Max));
    
    private readonly List<IntegerSelectorItemControl> integerControls = new();
    private ItemsControl? itemsControl;
    private IEnumerable<int>? selectedIntegers;
    private readonly List<IDisposable> disposables = new();
    
    static IntegersSelectorControl()
    {
        MaxProperty.Changed.AddClassHandler<IntegersSelectorControl>((control, _) => control.UpdateIntegers());
        MinProperty.Changed.AddClassHandler<IntegersSelectorControl>((control, _) => control.UpdateIntegers());
        
        SelectedIntegersProperty.Changed.AddClassHandler<IntegersSelectorControl>((control, _) =>
            control.UpdateSelectedIntegers());
    }
    
    public IEnumerable<int>? SelectedIntegers
    {
        get => GetValue(SelectedIntegersProperty);
        set => SetValue(SelectedIntegersProperty, value);
    }
    
    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }
    
    public int Min
    {
        get => GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        itemsControl = e.NameScope.Find<ItemsControl>("PART_ItemsControl");
        UpdateIntegers();
    }
    
    private void UpdateSelectedIntegers()
    {
        if (SelectedIntegers is null)
        {
            if (selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= SelectedIntegersChangedEventHandler;
            }
            
            selectedIntegers = null;
            
            foreach (var integerControl in integerControls)
            {
                if (integerControl.IsSelected)
                {
                    integerControl.IsSelected = false;
                }
            }
        }
        else
        {
            if (selectedIntegers != SelectedIntegers)
            {
                if (selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -= SelectedIntegersChangedEventHandler;
                }
                
                selectedIntegers = SelectedIntegers;
                
                if (selectedIntegers is INotifyCollectionChanged changed)
                {
                    changed.CollectionChanged += SelectedIntegersChangedEventHandler;
                }
            }
            
            foreach (var integerControl in integerControls)
            {
                var value = SelectedIntegers.Contains(integerControl.Value);
                
                if (value != integerControl.IsSelected)
                {
                    integerControl.IsSelected = value;
                }
            }
        }
    }
    
    private void SelectedIntegersChangedEventHandler(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not IEnumerable<int> enumerable)
        {
            return;
        }
        
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add: break;
            case NotifyCollectionChangedAction.Remove: break;
            case NotifyCollectionChangedAction.Replace: return;
            case NotifyCollectionChangedAction.Move: return;
            case NotifyCollectionChangedAction.Reset: return;
            default: throw new ArgumentOutOfRangeException();
        }
        
        foreach (var integerControl in integerControls)
        {
            var value = enumerable.Contains(integerControl.Value);
            
            if (value != integerControl.IsSelected)
            {
                integerControl.IsSelected = value;
            }
        }
    }
    
    private void Clear()
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }
        
        integerControls.Clear();
        itemsControl?.Items.Clear();
    }
    
    private void UpdateIntegers()
    {
        if (itemsControl is null)
        {
            return;
        }
        
        Clear();
        
        if (Min > Max)
        {
            return;
        }
        
        integerControls.AddRange(Enumerable.Range(Min, Max)
           .Select(x => new IntegerSelectorItemControl
            {
                Value = x,
            }));
        
        foreach (var integerControl in integerControls)
        {
            var source = integerControl.GetObservable(IntegerSelectorItemControl.IsSelectedProperty)
               .Skip(1)
               .Subscribe(x =>
                {
                    if (selectedIntegers is null)
                    {
                        return;
                    }
                    
                    if (x)
                    {
                        if (selectedIntegers.Contains(integerControl.Value))
                        {
                            return;
                        }
                        
                        if (selectedIntegers is ICollection<int> collection)
                        {
                            collection.Add(integerControl.Value);
                        }
                    }
                    else
                    {
                        if (!selectedIntegers.Contains(integerControl.Value))
                        {
                            return;
                        }
                        
                        if (selectedIntegers is ICollection<int> collection)
                        {
                            collection.Remove(integerControl.Value);
                        }
                    }
                });
            
            disposables.Add(source);
        }
        
        foreach (var integer in integerControls)
        {
            itemsControl.Items.Add(integer);
        }
        
        UpdateSelectedIntegers();
    }
}