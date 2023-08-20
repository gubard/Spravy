using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Spravy.Ui.Models;

namespace Spravy.Ui.Controls;

[TemplatePart(MondayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TuesdayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(WednesdayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ThursdayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(FridayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SaturdayCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SundayCheckBoxPartName, typeof(CheckBox))]
public class DayOfWeekSelector : TemplatedControl
{
    private readonly DayOfWeek[] DaysOfWeek = Enum.GetValues<DayOfWeek>();
    private readonly Dictionary<DayOfWeek, EventHandler<RoutedEventArgs>> eventHandlers = new();

    public const string MondayCheckBoxPartName = "PART_MondayCheckBox";
    public const string TuesdayCheckBoxPartName = "PART_TuesdayCheckBox";
    public const string WednesdayCheckBoxPartName = "PART_WednesdayCheckBox";
    public const string ThursdayCheckBoxPartName = "PART_ThursdayCheckBox";
    public const string FridayCheckBoxPartName = "PART_FridayCheckBox";
    public const string SaturdayCheckBoxPartName = "PART_SaturdayCheckBox";
    public const string SundayCheckBoxPartName = "PART_SundayCheckBox";

    private CheckBox? mondayCheckBox;
    private CheckBox? tuesdayCheckBox;
    private CheckBox? wednesdayCheckBox;
    private CheckBox? thursdayCheckBox;
    private CheckBox? fridayCheckBox;
    private CheckBox? saturdayCheckBox;
    private CheckBox? sundayCheckBox;

    public DayOfWeekSelector()
    {
        SelectedDaysOfWeek.CollectionChanged += SelectedDaysOfWeekChangedCore;
    }

    public AvaloniaList<DayOfWeek> SelectedDaysOfWeek { get; } = new();

    public event EventHandler<SelectedDaysOfWeekChangedEventArgs>? SelectedDaysOfWeekChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SubCheckBox(ref mondayCheckBox, e.NameScope, MondayCheckBoxPartName, DayOfWeek.Monday);
        SubCheckBox(ref tuesdayCheckBox, e.NameScope, TuesdayCheckBoxPartName, DayOfWeek.Tuesday);
        SubCheckBox(ref wednesdayCheckBox, e.NameScope, WednesdayCheckBoxPartName, DayOfWeek.Wednesday);
        SubCheckBox(ref thursdayCheckBox, e.NameScope, ThursdayCheckBoxPartName, DayOfWeek.Thursday);
        SubCheckBox(ref fridayCheckBox, e.NameScope, FridayCheckBoxPartName, DayOfWeek.Friday);
        SubCheckBox(ref saturdayCheckBox, e.NameScope, SaturdayCheckBoxPartName, DayOfWeek.Saturday);
        SubCheckBox(ref sundayCheckBox, e.NameScope, SundayCheckBoxPartName, DayOfWeek.Sunday);
        UpdateSelectedDaysOfWeekChanged(SelectedDaysOfWeek);
    }

    private void SafeChangeIsChecked(CheckBox checkBox, DayOfWeek dayOfWeek, bool isChecked)
    {
        var isCheckedChanged = GetEventHandler(dayOfWeek);
        checkBox.IsCheckedChanged -= isCheckedChanged;
        checkBox.IsChecked = isChecked;
        checkBox.IsCheckedChanged += isCheckedChanged;
    }

    private void SubCheckBox(ref CheckBox? checkBox, INameScope scope, string checkBoxName, DayOfWeek dayOfWeek)
    {
        var isCheckedChanged = GetEventHandler(dayOfWeek);

        if (checkBox is not null)
        {
            checkBox.IsCheckedChanged -= isCheckedChanged;
        }

        checkBox = scope.Get<CheckBox>(checkBoxName);
        checkBox.IsCheckedChanged += isCheckedChanged;
    }

    private EventHandler<RoutedEventArgs> GetEventHandler(DayOfWeek dayOfWeek)
    {
        if (eventHandlers.TryGetValue(dayOfWeek, out var handler))
        {
            return handler;
        }

        handler = (source, _) =>
        {
            if (source is not CheckBox checkBox)
            {
                return;
            }

            SelectedDaysOfWeek.CollectionChanged -= SelectedDaysOfWeekChangedCore;

            if (checkBox.IsChecked == true)
            {
                SafeAddSelectedDaysOfWeek(dayOfWeek);
            }
            else
            {
                SelectedDaysOfWeek.Remove(dayOfWeek);
            }

            var args = new SelectedDaysOfWeekChangedEventArgs(SelectedDaysOfWeek.ToArray());
            SelectedDaysOfWeekChanged?.Invoke(this, args);
            SelectedDaysOfWeek.CollectionChanged += SelectedDaysOfWeekChangedCore;
        };

        eventHandlers[dayOfWeek] = handler;

        return handler;
    }

    private void SafeAddSelectedDaysOfWeek(DayOfWeek dayOfWeek)
    {
        if (!SelectedDaysOfWeek.Contains(dayOfWeek))
        {
            SelectedDaysOfWeek.Add(dayOfWeek);
        }
    }

    private void SelectedDaysOfWeekChangedCore(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not AvaloniaList<DayOfWeek> list)
        {
            return;
        }

        UpdateSelectedDaysOfWeekChanged(list);
    }

    private void UpdateSelectedDaysOfWeekChanged(AvaloniaList<DayOfWeek> list)
    {
        if (mondayCheckBox is null
            || tuesdayCheckBox is null
            || wednesdayCheckBox is null
            || thursdayCheckBox is null
            || fridayCheckBox is null
            || saturdayCheckBox is null
            || sundayCheckBox is null)
        {
            return;
        }

        foreach (var dayOfWeek in DaysOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    SafeChangeIsChecked(sundayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Monday:
                    SafeChangeIsChecked(mondayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Tuesday:
                    SafeChangeIsChecked(tuesdayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Wednesday:
                    SafeChangeIsChecked(wednesdayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Thursday:
                    SafeChangeIsChecked(thursdayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Friday:
                    SafeChangeIsChecked(fridayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                case DayOfWeek.Saturday:
                    SafeChangeIsChecked(saturdayCheckBox, dayOfWeek, list.Contains(dayOfWeek));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}