namespace Spravy.Ui.Controls;

public class ViewSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ViewSelectorItemControl>> ItemsProperty =
        AvaloniaProperty.Register<ViewSelectorControl, IEnumerable<ViewSelectorItemControl>>(
            nameof(Items)
        );

    public static readonly StyledProperty<object?> StateProperty = AvaloniaProperty.Register<
        ViewSelectorControl,
        object?
    >(nameof(State));

    private Panel? panel;

    public ViewSelectorControl()
    {
        Items = new AvaloniaList<ViewSelectorItemControl>();
    }

    public object? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    [Content]
    public IEnumerable<ViewSelectorItemControl> Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        panel = e.NameScope.Find<Panel>("PART_Panel");
        base.OnApplyTemplate(e);

        if (panel is null)
        {
            return;
        }

        panel.Children.AddRange(Items);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == StateProperty)
        {
            foreach (var item in Items)
            {
                item.Update(State);
            }
        }
    }
}
