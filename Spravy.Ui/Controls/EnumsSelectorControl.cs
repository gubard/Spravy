namespace Spravy.Ui.Controls;

public class EnumsSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> SelectedEnumsProperty =
        AvaloniaProperty.Register<EnumsSelectorControl, IEnumerable?>(nameof(SelectedEnums));

    private readonly List<EnumSelectorItemControl> enumControls = new();
    private ItemsControl? itemsControl;
    private IEnumerable? selectedEnums;

    static EnumsSelectorControl()
    {
        SelectedEnumsProperty.Changed.AddClassHandler<EnumsSelectorControl>(
            (control, _) =>
            {
                control.UpdateEnums();
                control.UpdateSelectedEnums();
            }
        );
    }

    public IEnumerable? SelectedEnums
    {
        get => GetValue(SelectedEnumsProperty);
        set => SetValue(SelectedEnumsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        itemsControl = e.NameScope.Find<ItemsControl>("PART_ItemsControl");
        UpdateEnums();
        UpdateSelectedEnums();
    }

    private void UpdateSelectedEnums()
    {
        if (SelectedEnums is null)
        {
            if (selectedEnums is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= SelectedIntegersChangedEventHandler;
            }

            selectedEnums = null;

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
            if (!Equals(selectedEnums, SelectedEnums))
            {
                if (selectedEnums is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged -=
                        SelectedIntegersChangedEventHandler;
                }

                selectedEnums = SelectedEnums;

                if (selectedEnums is INotifyCollectionChanged changed)
                {
                    changed.CollectionChanged += SelectedIntegersChangedEventHandler;
                }
            }

            foreach (var integerControl in enumControls)
            {
                var value = SelectedEnums.OfType<object>().Contains(integerControl.Value);

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
        if (sender is not IEnumerable enumerable)
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

        var array = enumerable.OfType<object>().ToArray();

        foreach (var integerControl in enumControls)
        {
            var value = array.Contains(integerControl.Value);

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

    private void UpdateEnums()
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
                .Select(x => new EnumSelectorItemControl { Value = x })
                .ToArray()
        );

        foreach (var integerControl in enumControls)
        {
            integerControl.PropertyChanged += OnPropertyChanged;
        }

        foreach (var enumControl in enumControls)
        {
            itemsControl.Items.Add(enumControl);
        }

        UpdateSelectedEnums();
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not EnumSelectorItemControl integerControl)
        {
            return;
        }

        if (e.Property.Name == nameof(EnumSelectorItemControl.IsSelected))
        {
            if (selectedEnums is null)
            {
                return;
            }

            if (integerControl.IsSelected)
            {
                if (selectedEnums.OfType<object>().Contains(integerControl.Value))
                {
                    return;
                }

                if (selectedEnums is IList collection)
                {
                    collection.Add(integerControl.Value);
                }
            }
            else
            {
                if (!selectedEnums.OfType<object>().Contains(integerControl.Value))
                {
                    return;
                }

                if (selectedEnums is IList collection)
                {
                    collection.Remove(integerControl.Value);
                }
            }
        }
    }
}
