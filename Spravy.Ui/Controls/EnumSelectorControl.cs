using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class EnumSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<ValueType?> SelectedEnumProperty =
        AvaloniaProperty.Register<EnumSelectorControl, ValueType?>(
            nameof(SelectedEnum),
            defaultBindingMode: BindingMode.TwoWay
        );

    private ListBox? listBox;

    static EnumSelectorControl()
    {
        SelectedEnumProperty.Changed.AddClassHandler<EnumSelectorControl>(
            (control, _) => control.UpdateEnums()
        );
    }

    public ValueType? SelectedEnum
    {
        get => GetValue(SelectedEnumProperty);
        set => SetValue(SelectedEnumProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<ListBox>("PART_ListBox");
        UpdateListBox(lb);
    }

    private void UpdateListBox(ListBox? lb)
    {
        if (listBox is not null)
        {
            listBox.SelectionChanged -= OnSelectionChanged;
        }

        listBox = lb;

        if (listBox is not null)
        {
            listBox.SelectionChanged += OnSelectionChanged;
        }

        UpdateEnums();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is not ListBox lb)
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
        if (listBox is null)
        {
            return;
        }

        if (SelectedEnum is null)
        {
            if (listBox.Items.Count != 0)
            {
                listBox.Items.Clear();
            }

            return;
        }

        var first = listBox.Items.FirstOrDefault();

        if (first is null)
        {
            var values = Enum.GetValues(SelectedEnum.GetType());

            foreach (var value in values)
            {
                listBox.Items.Add(value);
            }

            listBox.SelectedItem = SelectedEnum;

            return;
        }

        if (first.GetType() != SelectedEnum.GetType())
        {
            listBox.Items.Clear();
            var values = Enum.GetValues(SelectedEnum.GetType());

            foreach (var value in values)
            {
                listBox.Items.Add(value);
            }

            listBox.SelectedItem = SelectedEnum;

            return;
        }

        if (!Equals(listBox.SelectedItem, SelectedEnum))
        {
            listBox.SelectedItem = SelectedEnum;
        }
    }
}
