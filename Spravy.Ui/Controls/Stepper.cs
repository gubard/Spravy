using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class Stepper : TemplatedControl
{
    public static readonly StyledProperty<uint> IndexProperty = AvaloniaProperty.Register<
        EnumSelectorControl,
        uint
    >(nameof(Index), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable> StepsProperty = AvaloniaProperty.Register<
        EnumSelectorControl,
        IEnumerable
    >(nameof(Steps), defaultBindingMode: BindingMode.TwoWay);

    public IEnumerable Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    public uint Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }
}
