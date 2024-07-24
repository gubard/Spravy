using Avalonia.Data;
using Material.Icons.Avalonia;

namespace Spravy.Ui.Controls;

public class PathControl : TemplatedControl
{
    private static readonly FuncTemplate<Control?> defaultSeparator =
        new(() => new MaterialIcon { Kind = MaterialIconKind.ChevronRight, });

    public static readonly StyledProperty<ITemplate<Control?>> SeparatorProperty =
        AvaloniaProperty.Register<PathControl, ITemplate<Control?>>(
            nameof(SeparatorPanel),
            defaultSeparator
        );

    public static readonly StyledProperty<IEnumerable> SegmentsProperty = AvaloniaProperty.Register<
        PathControl,
        IEnumerable
    >(nameof(Segments), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable> ItemsProperty = AvaloniaProperty.Register<
        PathControl,
        IEnumerable
    >(nameof(Items), defaultBindingMode: BindingMode.OneWayToSource);

    static PathControl()
    {
        SegmentsProperty.Changed.AddClassHandler<PathControl>(
            (control, _) =>
            {
                var segments = control.Segments.OfType<object>().ToArray().AsSpan();
                var body = segments[..^1];
                var items = new object[segments.Length * 2 - 1].AsSpan();

                for (var i = 0; i < body.Length; i++)
                {
                    items[i * 2] = body[i];
                    items[i * 2 + 1] = control.SeparatorPanel.Build() ?? new object();
                }

                items[^1] = segments[^1];
                control.Items = items.ToArray();
            }
        );
    }

    public PathControl()
    {
        Items = Array.Empty<object>();
    }

    public ITemplate<Control?> SeparatorPanel
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    public IEnumerable Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public IEnumerable Items
    {
        get => GetValue(ItemsProperty);
        private set => SetValue(ItemsProperty, value);
    }
}
