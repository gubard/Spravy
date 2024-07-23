using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class Stepper : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> StepsProperty = AvaloniaProperty.Register<
        Stepper,
        IEnumerable
    >(nameof(Steps), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable> ItemsProperty = AvaloniaProperty.Register<
        Stepper,
        IEnumerable
    >(nameof(Items), defaultBindingMode: BindingMode.OneWayToSource);

    static Stepper()
    {
        StepsProperty.Changed.AddClassHandler<Stepper>(
            (control, _) => control.Items = control.Steps.OfType<object>().ToArray()
        );
    }

    public Stepper()
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
