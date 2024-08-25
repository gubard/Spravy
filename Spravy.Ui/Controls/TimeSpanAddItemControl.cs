namespace Spravy.Ui.Controls;

[TemplatePart(Name = "PART_TimePicker", Type = typeof(TimePicker))]
public class TimeSpanAddItemControl : AddItemControl
{
    private TimePicker? timePicker;

    public override object Value => timePicker?.SelectedTime ?? TimeSpan.Zero;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        timePicker = e.NameScope.Find<TimePicker>("PART_TimePicker");

        if (timePicker is not null)
        {
            timePicker.SelectedTime = TimeSpan.FromHours(1);
        }

        base.OnApplyTemplate(e);
    }
}
