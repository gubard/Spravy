namespace Spravy.Ui.Controls;

public class ViewSelectorItemControl : TemplatedControl
{
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<ViewSelectorItemControl, object?>(nameof(Content));

    public static readonly StyledProperty<object?> StateProperty =
        AvaloniaProperty.Register<ViewSelectorItemControl, object?>(nameof(State));

    private ContentControl? contentControl;

    public object? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        contentControl = e.NameScope.Find<ContentControl>("PART_ContentControl");
        base.OnApplyTemplate(e);
        Update(State);
    }

    public void Update(object? currentState)
    {
        if (contentControl is null)
        {
            return;
        }

        if (currentState is null && State is null)
        {
            contentControl.IsVisible = true;

            return;
        }

        if (currentState is null)
        {
            contentControl.IsVisible = false;

            return;
        }

        if (State is null)
        {
            contentControl.IsVisible = false;

            return;
        }

        if (currentState.Equals(State))
        {
            contentControl.IsVisible = true;

            return;
        }

        contentControl.IsVisible = false;
    }
}