using Avalonia.Metadata;

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

    static ViewSelectorControl()
    {
        StateProperty.Changed.AddClassHandler<ViewSelectorControl>(
            (c, a) =>
            {
                foreach (var item in c.Items)
                {
                    item.Update(c.State);
                }
            }
        );
    }

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
}
