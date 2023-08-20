using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Controls;

[TemplatePart(JanuaryDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(FebruaryDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(MarchDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(AprilDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(MayDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(JuneDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(JulyDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(AugustDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(SeptemberDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(OctoberDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(NovemberDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
[TemplatePart(DecemberDayOfMonthSelectorPartName, typeof(DayOfMonthSelector))]
public class DayOfYearSelector : TemplatedControl
{
    private readonly Dictionary<byte, NotifyCollectionChangedEventHandler> eventHandlers = new();

    public const string JanuaryDayOfMonthSelectorPartName = "PART_JanuaryDayOfMonthSelector";
    public const string FebruaryDayOfMonthSelectorPartName = "PART_FebruaryDayOfMonthSelector";
    public const string MarchDayOfMonthSelectorPartName = "PART_MarchDayOfMonthSelector";
    public const string AprilDayOfMonthSelectorPartName = "PART_AprilDayOfMonthSelector";
    public const string MayDayOfMonthSelectorPartName = "PART_MayDayOfMonthSelector";
    public const string JuneDayOfMonthSelectorPartName = "PART_JuneDayOfMonthSelector";
    public const string JulyDayOfMonthSelectorPartName = "PART_JulyDayOfMonthSelector";
    public const string AugustDayOfMonthSelectorPartName = "PART_AugustDayOfMonthSelector";
    public const string SeptemberDayOfMonthSelectorPartName = "PART_SeptemberDayOfMonthSelector";
    public const string OctoberDayOfMonthSelectorPartName = "PART_OctoberDayOfMonthSelector";
    public const string NovemberDayOfMonthSelectorPartName = "PART_NovemberDayOfMonthSelector";
    public const string DecemberDayOfMonthSelectorPartName = "PART_DecemberDayOfMonthSelector";

    private DayOfMonthSelector? januaryDayOfMonthSelector;
    private DayOfMonthSelector? februaryDayOfMonthSelector;
    private DayOfMonthSelector? marchDayOfMonthSelector;
    private DayOfMonthSelector? aprilDayOfMonthSelector;
    private DayOfMonthSelector? mayDayOfMonthSelector;
    private DayOfMonthSelector? juneDayOfMonthSelector;
    private DayOfMonthSelector? julyDayOfMonthSelector;
    private DayOfMonthSelector? augustDayOfMonthSelector;
    private DayOfMonthSelector? septemberDayOfMonthSelector;
    private DayOfMonthSelector? octoberDayOfMonthSelector;
    private DayOfMonthSelector? novemberDayOfMonthSelector;
    private DayOfMonthSelector? decemberDayOfMonthSelector;

    public AvaloniaList<DayOfYear> SelectedDaysOfYear { get; } = new();

    public event EventHandler<SelectedSelectedDaysOfYearEventArgs>? SelectedDaysOfYearChanged;

    public DayOfYearSelector()
    {
        SelectedDaysOfYear.CollectionChanged += SelectedDaysOfYearChangedCore;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SubDayOfMonthSelector(ref januaryDayOfMonthSelector, e.NameScope, JanuaryDayOfMonthSelectorPartName, 1);
        SubDayOfMonthSelector(ref februaryDayOfMonthSelector, e.NameScope, FebruaryDayOfMonthSelectorPartName, 2);
        SubDayOfMonthSelector(ref marchDayOfMonthSelector, e.NameScope, MarchDayOfMonthSelectorPartName, 3);
        SubDayOfMonthSelector(ref aprilDayOfMonthSelector, e.NameScope, AprilDayOfMonthSelectorPartName, 4);
        SubDayOfMonthSelector(ref mayDayOfMonthSelector, e.NameScope, MayDayOfMonthSelectorPartName, 5);
        SubDayOfMonthSelector(ref juneDayOfMonthSelector, e.NameScope, JuneDayOfMonthSelectorPartName, 6);
        SubDayOfMonthSelector(ref julyDayOfMonthSelector, e.NameScope, JulyDayOfMonthSelectorPartName, 7);
        SubDayOfMonthSelector(ref augustDayOfMonthSelector, e.NameScope, AugustDayOfMonthSelectorPartName, 8);
        SubDayOfMonthSelector(ref septemberDayOfMonthSelector, e.NameScope, SeptemberDayOfMonthSelectorPartName, 9);
        SubDayOfMonthSelector(ref octoberDayOfMonthSelector, e.NameScope, OctoberDayOfMonthSelectorPartName, 10);
        SubDayOfMonthSelector(ref novemberDayOfMonthSelector, e.NameScope, NovemberDayOfMonthSelectorPartName, 11);
        SubDayOfMonthSelector(ref decemberDayOfMonthSelector, e.NameScope, DecemberDayOfMonthSelectorPartName, 12);
        UpdateSelectedDaysOfYear(SelectedDaysOfYear);
    }

    private void SubDayOfMonthSelector(
        ref DayOfMonthSelector? dayOfMonthSelector,
        INameScope scope,
        string dayOfMonthSelectorName,
        byte month
    )
    {
        var selectedDaysOfMonthChanged = GetEventHandler(month);

        if (dayOfMonthSelector is not null)
        {
            dayOfMonthSelector.SelectedDaysOfMonth.CollectionChanged -= selectedDaysOfMonthChanged;
        }

        dayOfMonthSelector = scope.Get<DayOfMonthSelector>(dayOfMonthSelectorName);
        dayOfMonthSelector.SelectedDaysOfMonth.CollectionChanged += selectedDaysOfMonthChanged;
    }

    private NotifyCollectionChangedEventHandler GetEventHandler(byte month)
    {
        if (eventHandlers.TryGetValue(month, out var handler))
        {
            return handler;
        }

        handler = (_, _) =>
        {
            SelectedDaysOfYear.CollectionChanged -= SelectedDaysOfYearChangedCore;
            var dayOfMonthSelector = GetDayOfMonthSelector(month);
            SelectedDaysOfYear.RemoveAll(SelectedDaysOfYear.Where(x => x.Month == month));
            SelectedDaysOfYear.AddRange(dayOfMonthSelector.SelectedDaysOfMonth.Select(x => new DayOfYear(x, month)));
            var args = new SelectedSelectedDaysOfYearEventArgs(SelectedDaysOfYear.ToArray());
            SelectedDaysOfYearChanged?.Invoke(this, args);
            SelectedDaysOfYear.CollectionChanged += SelectedDaysOfYearChangedCore;
        };

        eventHandlers[month] = handler;

        return handler;
    }

    private void SelectedDaysOfYearChangedCore(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not AvaloniaList<DayOfYear> list)
        {
            return;
        }

        UpdateSelectedDaysOfYear(list);
    }

    private void UpdateSelectedDaysOfYear(AvaloniaList<DayOfYear> list)
    {
        if (januaryDayOfMonthSelector is null
            || februaryDayOfMonthSelector is null
            || marchDayOfMonthSelector is null
            || aprilDayOfMonthSelector is null
            || mayDayOfMonthSelector is null
            || juneDayOfMonthSelector is null
            || julyDayOfMonthSelector is null
            || augustDayOfMonthSelector is null
            || septemberDayOfMonthSelector is null
            || octoberDayOfMonthSelector is null
            || novemberDayOfMonthSelector is null
            || decemberDayOfMonthSelector is null)
        {
            return;
        }

        for (byte i = 1; i <= 12; i++)
        {
            var dayOfMonthSelector = GetDayOfMonthSelector(i);
            dayOfMonthSelector.SelectedDaysOfMonth.Clear();
            dayOfMonthSelector.SelectedDaysOfMonth.AddRange(list.Where(x => x.Month == i).Select(x => x.Day));
        }
    }

    private DayOfMonthSelector GetDayOfMonthSelector(byte month)
    {
        switch (month)
        {
            case 1: return januaryDayOfMonthSelector;
            case 2: return februaryDayOfMonthSelector;
            case 3: return marchDayOfMonthSelector;
            case 4: return aprilDayOfMonthSelector;
            case 5: return mayDayOfMonthSelector;
            case 6: return juneDayOfMonthSelector;
            case 7: return julyDayOfMonthSelector;
            case 8: return augustDayOfMonthSelector;
            case 9: return septemberDayOfMonthSelector;
            case 10: return octoberDayOfMonthSelector;
            case 11: return novemberDayOfMonthSelector;
            case 12: return decemberDayOfMonthSelector;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}