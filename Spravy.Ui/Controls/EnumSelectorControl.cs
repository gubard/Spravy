using Spravy.Ui.Converters;

namespace Spravy.Ui.Controls;

public class EnumSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<ValueType?> SelectedEnumProperty =
        AvaloniaProperty.Register<EnumSelectorControl, ValueType?>(
            nameof(SelectedEnum),
            defaultBindingMode: BindingMode.TwoWay
        );

    private SelectingItemsControl? selectingItemsControl;

    static EnumSelectorControl()
    {
        SelectedEnumProperty.Changed.AddClassHandler<EnumSelectorControl>((control, _) => control.UpdateEnums());
    }

    public ValueType? SelectedEnum
    {
        get => GetValue(SelectedEnumProperty);
        set => SetValue(SelectedEnumProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        UpdateSelectingItemsControl(lb);
    }

    private void UpdateSelectingItemsControl(SelectingItemsControl? lb)
    {
        if (selectingItemsControl is not null)
        {
            selectingItemsControl.SelectionChanged -= OnSelectionChanged;
        }

        selectingItemsControl = lb;

        if (selectingItemsControl is not null)
        {
            selectingItemsControl.SelectionChanged += OnSelectionChanged;

            selectingItemsControl.DisplayMemberBinding = new Binding
            {
                Converter = EnumLocalizationValueConverter.Default,
            };
        }

        UpdateEnums();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is not SelectingItemsControl lb)
        {
            return;
        }

        if (lb.SelectedItem is not ValueType vt)
        {
            return;
        }

        if (!Equals(SelectedEnum, vt))
        {
            SelectedEnum = vt;
        }
    }

    private void UpdateEnums()
    {
        if (selectingItemsControl is null)
        {
            return;
        }

        if (SelectedEnum is null)
        {
            if (selectingItemsControl.Items.Count != 0)
            {
                selectingItemsControl.Items.Clear();
            }

            return;
        }

        var first = selectingItemsControl.Items.FirstOrDefault();

        if (first is null)
        {
            var values = UiHelper.GetEnumValues(SelectedEnum.GetType());

            foreach (var value in values.Span)
            {
                selectingItemsControl.Items.Add(value);
            }

            selectingItemsControl.SelectedItem = SelectedEnum;

            return;
        }

        if (first.GetType() != SelectedEnum.GetType())
        {
            selectingItemsControl.Items.Clear();
            var values = UiHelper.GetEnumValues(SelectedEnum.GetType());

            foreach (var value in values.Span)
            {
                selectingItemsControl.Items.Add(value);
            }

            selectingItemsControl.SelectedItem = SelectedEnum;

            return;
        }

        if (!Equals(selectingItemsControl.SelectedItem, SelectedEnum))
        {
            selectingItemsControl.SelectedItem = SelectedEnum;
        }
    }
}