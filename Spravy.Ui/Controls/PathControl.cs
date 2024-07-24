using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class PathControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> StepsProperty = AvaloniaProperty.Register<
        PathControl,
        IEnumerable
    >(nameof(Steps), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable> ItemsProperty = AvaloniaProperty.Register<
        PathControl,
        IEnumerable
    >(nameof(Items), defaultBindingMode: BindingMode.OneWayToSource);

    static PathControl()
    {
        StepsProperty.Changed.AddClassHandler<PathControl>(
            (control, _) => control.Items = control.Steps.OfType<object>().ToArray()
        );
    }

    public PathControl()
    {
        Items = Array.Empty<object>();
    }

    public IEnumerable Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    public IEnumerable Items
    {
        get => GetValue(ItemsProperty);
        private set => SetValue(ItemsProperty, value);
    }
}
