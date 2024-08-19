namespace Spravy.Ui.Controls;

public class EnumsSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ValueType>?> SelectedEnumsProperty =
        AvaloniaProperty.Register<EnumsSelectorControl, IEnumerable<ValueType>?>(
            nameof(SelectedEnums)
        );

    private readonly List<EnumSelectorItemControl> enumControls = new();
    private ItemsControl? itemsControl;
    private IEnumerable<ValueType>? selectedIntegers;

    static EnumsSelectorControl()
    {
        SelectedEnumsProperty.Changed.AddClassHandler<EnumsSelectorControl>(
            (control, _) => control.UpdateSelectedIntegers()
        );
    }

    public IEnumerable<ValueType>? SelectedEnums
    {
        get => GetValue(SelectedEnumsProperty);
        set => SetValue(SelectedEnumsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        itemsControl = e.NameScope.Find<ItemsControl>("PART_ItemsControl");
        UpdateIntegers();
    }

    private void UpdateSelectedIntegers()
    {
        if (SelectedEnums is null)
        {
            if (selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= SelectedIntegersChangedEventHandler;
            }

            selectedIntegers = null;

            foreach (var integerControl in enumControls)
            {
                if (integerControl.IsSelected)
                {
                    integerControl.IsSelected = false;
                }
            }
        }
        else
        {
            if (selectedIntegers != SelectedEnums)
            {
                if (selectedIntegers is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -=
                        SelectedIntegersChangedEventHandler;
                }

                selectedIntegers = SelectedEnums;

                if (selectedIntegers is INotifyCollectionChanged changed)
                {
                    changed.CollectionChanged += SelectedIntegersChangedEventHandler;
                }
            }

            foreach (var integerControl in enumControls)
            {
                var value = SelectedEnums.Contains(integerControl.Value);

                if (value != integerControl.IsSelected)
                {
                    integerControl.IsSelected = value;
                }
            }
        }
    }

    private void SelectedIntegersChangedEventHandler(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        if (sender is not IEnumerable<ValueType> enumerable)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                break;
            case NotifyCollectionChangedAction.Remove:
                break;
            case NotifyCollectionChangedAction.Replace:
                return;
            case NotifyCollectionChangedAction.Move:
                return;
            case NotifyCollectionChangedAction.Reset:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var integerControl in enumControls)
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
        foreach (var integerControl in enumControls)
        {
            integerControl.PropertyChanged -= OnPropertyChanged;
        }

        enumControls.Clear();
        itemsControl?.Items.Clear();
    }

    private void UpdateIntegers()
    {
        if (SelectedEnums is null)
        {
            return;
        }

        if (itemsControl is null)
        {
            return;
        }

        Clear();
        enumControls.AddRange(
            UiHelper
                .GetEnumValues(SelectedEnums.GetType().GetGenericArguments()[0])
                .Select(x => new EnumSelectorItemControl { Value = (ValueType)x, })
                .ToArray()
        );

        foreach (var integerControl in enumControls)
        {
            integerControl.PropertyChanged += OnPropertyChanged;
        }

        foreach (var integer in enumControls)
        {
            itemsControl.Items.Add(integer);
        }

        UpdateSelectedIntegers();
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not EnumSelectorItemControl integerControl)
        {
            return;
        }

        if (e.Property.Name == nameof(EnumSelectorItemControl.IsSelected))
        {
            if (selectedIntegers is null)
            {
                return;
            }

            if (integerControl.IsSelected)
            {
                if (selectedIntegers.Contains(integerControl.Value))
                {
                    return;
                }

                if (selectedIntegers is ICollection<ValueType> collection)
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

                if (selectedIntegers is ICollection<ValueType> collection)
                {
                    collection.Remove(integerControl.Value);
                }
            }
        }
    }
}
