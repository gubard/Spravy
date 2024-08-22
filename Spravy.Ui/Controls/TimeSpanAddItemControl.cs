namespace Spravy.Ui.Controls;

public class TimeSpanAddItemControl : AddItemControl
{
    public static readonly StyledProperty<TimeSpan> TimeProperty = AvaloniaProperty.Register<
        TimeSpanAddItemControl,
        TimeSpan
    >(nameof(Time));

    public override object Value => Time;

    public TimeSpan Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }
}
