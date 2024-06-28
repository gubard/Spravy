namespace Spravy.Ui.Controls;

[TemplatePart(ClearButtonPartName, typeof(Button))]
[TemplatePart(DatePickerPartName, typeof(DatePicker))]
public class NullableDatePicker : TemplatedControl
{
    public const string ClearButtonPartName = "PART_ClearButton";
    public const string DatePickerPartName = "PART_DatePicker";

    /// <summary>
    ///     Defines the <see cref="SelectedDate" /> Property
    /// </summary>
    public static readonly StyledProperty<DateTimeOffset?> SelectedDateProperty =
        DatePicker.SelectedDateProperty.AddOwner<NullableDatePicker>();

    private Button? clearButton;
    private DatePicker? datePicker;

    /// <summary>
    ///     Gets or sets the Selected Date for the picker, can be null
    /// </summary>
    public DateTimeOffset? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (clearButton is not null)
        {
            clearButton.Click -= ClearDate;
        }

        clearButton = e.NameScope.Get<Button>(ClearButtonPartName);
        datePicker = e.NameScope.Get<DatePicker>(DatePickerPartName);
        clearButton.Click += ClearDate;
    }

    private void ClearDate(object? sender, RoutedEventArgs e)
    {
        if (datePicker is null)
        {
            return;
        }

        datePicker.SelectedDate = null;
    }
}
