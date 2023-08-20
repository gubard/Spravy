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

[TemplatePart(OneCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwoCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ThreeCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(FourCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(FiveCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SixCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SevenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(EightCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(NineCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ElevenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwelveCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ThirteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(FourteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(FifteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SixteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(SeventeenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(EighteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(NineteenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyOneCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyTwoCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyThreeCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyFourCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyFiveCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentySixCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentySevenCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyEightCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(TwentyNineCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ThirtyCheckBoxPartName, typeof(CheckBox))]
[TemplatePart(ThirtyOneCheckBoxPartName, typeof(CheckBox))]
public class DayOfMonthSelector : TemplatedControl
{
    private readonly byte[] DaysOfMonth =
    {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27,
        28,
        29,
        30,
        31,
    };

    private readonly Dictionary<byte, EventHandler<RoutedEventArgs>> eventHandlers = new();

    public const string OneCheckBoxPartName = "PART_OneCheckBox";
    public const string TwoCheckBoxPartName = "PART_TwoCheckBox";
    public const string ThreeCheckBoxPartName = "PART_ThreeCheckBox";
    public const string FourCheckBoxPartName = "PART_FourCheckBox";
    public const string FiveCheckBoxPartName = "PART_FiveCheckBox";
    public const string SixCheckBoxPartName = "PART_SixCheckBox";
    public const string SevenCheckBoxPartName = "PART_SevenCheckBox";
    public const string EightCheckBoxPartName = "PART_EightCheckBox";
    public const string NineCheckBoxPartName = "PART_NineCheckBox";
    public const string TenCheckBoxPartName = "PART_TenCheckBox";
    public const string ElevenCheckBoxPartName = "PART_ElevenCheckBox";
    public const string TwelveCheckBoxPartName = "PART_TwelveCheckBox";
    public const string ThirteenCheckBoxPartName = "PART_ThirteenCheckBox";
    public const string FourteenCheckBoxPartName = "PART_FourteenCheckBox";
    public const string FifteenCheckBoxPartName = "PART_FifteenCheckBox";
    public const string SixteenCheckBoxPartName = "PART_SixteenCheckBox";
    public const string SeventeenCheckBoxPartName = "PART_SeventeenCheckBox";
    public const string EighteenCheckBoxPartName = "PART_EighteenCheckBox";
    public const string NineteenCheckBoxPartName = "PART_NineteenCheckBox";
    public const string TwentyCheckBoxPartName = "PART_TwentyCheckBox";
    public const string TwentyOneCheckBoxPartName = "PART_TwentyOneCheckBox";
    public const string TwentyTwoCheckBoxPartName = "PART_TwentyTwoCheckBox";
    public const string TwentyThreeCheckBoxPartName = "PART_TwentyThreeCheckBox";
    public const string TwentyFourCheckBoxPartName = "PART_TwentyFourCheckBox";
    public const string TwentyFiveCheckBoxPartName = "PART_TwentyFiveCheckBox";
    public const string TwentySixCheckBoxPartName = "PART_TwentySixCheckBox";
    public const string TwentySevenCheckBoxPartName = "PART_TwentySevenCheckBox";
    public const string TwentyEightCheckBoxPartName = "PART_TwentyEightCheckBox";
    public const string TwentyNineCheckBoxPartName = "PART_TwentyNineCheckBox";
    public const string ThirtyCheckBoxPartName = "PART_ThirtyCheckBox";
    public const string ThirtyOneCheckBoxPartName = "PART_ThirtyOneCheckBox";

    private CheckBox? oneCheckBox;
    private CheckBox? twoCheckBox;
    private CheckBox? threeCheckBox;
    private CheckBox? fourCheckBox;
    private CheckBox? fiveCheckBox;
    private CheckBox? sixCheckBox;
    private CheckBox? sevenCheckBox;
    private CheckBox? eightCheckBox;
    private CheckBox? nineCheckBox;
    private CheckBox? tenCheckBox;
    private CheckBox? elevenCheckBox;
    private CheckBox? twelveCheckBox;
    private CheckBox? thirteenCheckBox;
    private CheckBox? fourteenCheckBox;
    private CheckBox? fifteenCheckBox;
    private CheckBox? sixteenCheckBox;
    private CheckBox? seventeenCheckBox;
    private CheckBox? eighteenCheckBox;
    private CheckBox? nineteenCheckBox;
    private CheckBox? twentyCheckBox;
    private CheckBox? twentyOneCheckBox;
    private CheckBox? twentyTwoCheckBox;
    private CheckBox? twentyThreeCheckBox;
    private CheckBox? twentyFourCheckBox;
    private CheckBox? twentyFiveCheckBox;
    private CheckBox? twentySixCheckBox;
    private CheckBox? twentySevenCheckBox;
    private CheckBox? twentyEightCheckBox;
    private CheckBox? twentyNineCheckBox;
    private CheckBox? thirtyCheckBox;
    private CheckBox? thirtyOneCheckBox;

    public AvaloniaList<byte> SelectedDaysOfMonth { get; } = new();

    public event EventHandler<SelectedDaysOfMonthChangedEventArgs>? SelectedDaysOfMonthChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SubCheckBox(ref oneCheckBox, e.NameScope, OneCheckBoxPartName, 1);
        SubCheckBox(ref twoCheckBox, e.NameScope, TwoCheckBoxPartName, 2);
        SubCheckBox(ref threeCheckBox, e.NameScope, ThreeCheckBoxPartName, 3);
        SubCheckBox(ref fourCheckBox, e.NameScope, FourCheckBoxPartName, 4);
        SubCheckBox(ref fiveCheckBox, e.NameScope, FiveCheckBoxPartName, 5);
        SubCheckBox(ref sixCheckBox, e.NameScope, SixCheckBoxPartName, 6);
        SubCheckBox(ref sevenCheckBox, e.NameScope, SevenCheckBoxPartName, 7);
        SubCheckBox(ref eightCheckBox, e.NameScope, EightCheckBoxPartName, 8);
        SubCheckBox(ref nineCheckBox, e.NameScope, NineCheckBoxPartName, 9);
        SubCheckBox(ref tenCheckBox, e.NameScope, TenCheckBoxPartName, 10);
        SubCheckBox(ref elevenCheckBox, e.NameScope, ElevenCheckBoxPartName, 11);
        SubCheckBox(ref twelveCheckBox, e.NameScope, TwelveCheckBoxPartName, 12);
        SubCheckBox(ref thirteenCheckBox, e.NameScope, ThirteenCheckBoxPartName, 13);
        SubCheckBox(ref fourteenCheckBox, e.NameScope, FourteenCheckBoxPartName, 14);
        SubCheckBox(ref fifteenCheckBox, e.NameScope, FifteenCheckBoxPartName, 15);
        SubCheckBox(ref sixteenCheckBox, e.NameScope, SixteenCheckBoxPartName, 16);
        SubCheckBox(ref seventeenCheckBox, e.NameScope, SeventeenCheckBoxPartName, 17);
        SubCheckBox(ref eighteenCheckBox, e.NameScope, EighteenCheckBoxPartName, 18);
        SubCheckBox(ref nineteenCheckBox, e.NameScope, NineteenCheckBoxPartName, 19);
        SubCheckBox(ref twentyCheckBox, e.NameScope, TwentyCheckBoxPartName, 20);
        SubCheckBox(ref twentyOneCheckBox, e.NameScope, TwentyOneCheckBoxPartName, 21);
        SubCheckBox(ref twentyTwoCheckBox, e.NameScope, TwentyTwoCheckBoxPartName, 22);
        SubCheckBox(ref twentyThreeCheckBox, e.NameScope, TwentyThreeCheckBoxPartName, 23);
        SubCheckBox(ref twentyFourCheckBox, e.NameScope, TwentyFourCheckBoxPartName, 24);
        SubCheckBox(ref twentyFiveCheckBox, e.NameScope, TwentyFiveCheckBoxPartName, 25);
        SubCheckBox(ref twentySixCheckBox, e.NameScope, TwentySixCheckBoxPartName, 26);
        SubCheckBox(ref twentySevenCheckBox, e.NameScope, TwentySevenCheckBoxPartName, 27);
        SubCheckBox(ref twentyEightCheckBox, e.NameScope, TwentyEightCheckBoxPartName, 28);
        SubCheckBox(ref twentyNineCheckBox, e.NameScope, TwentyNineCheckBoxPartName, 29);
        SubCheckBox(ref thirtyCheckBox, e.NameScope, ThirtyCheckBoxPartName, 30);
        SubCheckBox(ref thirtyOneCheckBox, e.NameScope, ThirtyOneCheckBoxPartName, 31);
        UpdateSelectedDaysOfMonth(SelectedDaysOfMonth);
    }

    private void SafeChangeIsChecked(CheckBox checkBox, byte dayOfMonth, bool isChecked)
    {
        var isCheckedChanged = GetEventHandler(dayOfMonth);
        checkBox.IsCheckedChanged -= isCheckedChanged;
        checkBox.IsChecked = isChecked;
        checkBox.IsCheckedChanged += isCheckedChanged;
    }

    private void SubCheckBox(ref CheckBox? checkBox, INameScope scope, string checkBoxName, byte dayOfMonth)
    {
        var isCheckedChanged = GetEventHandler(dayOfMonth);

        if (checkBox is not null)
        {
            checkBox.IsCheckedChanged -= isCheckedChanged;
        }

        checkBox = scope.Get<CheckBox>(checkBoxName);
        checkBox.IsCheckedChanged += isCheckedChanged;
    }

    private EventHandler<RoutedEventArgs> GetEventHandler(byte dayOfMonth)
    {
        if (eventHandlers.TryGetValue(dayOfMonth, out var handler))
        {
            return handler;
        }

        handler = (source, _) =>
        {
            if (source is not CheckBox checkBox)
            {
                return;
            }

            SelectedDaysOfMonth.CollectionChanged -= SelectedDaysOfMonthChangedCore;

            if (checkBox.IsChecked == true)
            {
                SafeAddSelectedDaysOfMonth(dayOfMonth);
            }
            else
            {
                SelectedDaysOfMonth.Remove(dayOfMonth);
            }

            var args = new SelectedDaysOfMonthChangedEventArgs(SelectedDaysOfMonth.ToArray());
            SelectedDaysOfMonthChanged?.Invoke(this, args);
            SelectedDaysOfMonth.CollectionChanged += SelectedDaysOfMonthChangedCore;
        };

        eventHandlers[dayOfMonth] = handler;

        return handler;
    }

    private void SafeAddSelectedDaysOfMonth(byte dayOfMonth)
    {
        if (!SelectedDaysOfMonth.Contains(dayOfMonth))
        {
            SelectedDaysOfMonth.Add(dayOfMonth);
        }
    }

    private void SelectedDaysOfMonthChangedCore(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not AvaloniaList<byte> list)
        {
            return;
        }

        UpdateSelectedDaysOfMonth(list);
    }

    private void UpdateSelectedDaysOfMonth(AvaloniaList<byte> list)
    {
        if (oneCheckBox is null
            || twoCheckBox is null
            || threeCheckBox is null
            || fourCheckBox is null
            || fiveCheckBox is null
            || sixCheckBox is null
            || sevenCheckBox is null
            || eightCheckBox is null
            || nineCheckBox is null
            || tenCheckBox is null
            || elevenCheckBox is null
            || twelveCheckBox is null
            || thirteenCheckBox is null
            || fourteenCheckBox is null
            || fifteenCheckBox is null
            || sixteenCheckBox is null
            || seventeenCheckBox is null
            || eighteenCheckBox is null
            || nineteenCheckBox is null
            || twentyCheckBox is null
            || twentyOneCheckBox is null
            || twentyTwoCheckBox is null
            || twentyThreeCheckBox is null
            || twentyFourCheckBox is null
            || twentyFiveCheckBox is null
            || twentySixCheckBox is null
            || twentySevenCheckBox is null
            || twentyEightCheckBox is null
            || twentyNineCheckBox is null
            || thirtyCheckBox is null
            || thirtyOneCheckBox is null)
        {
            return;
        }

        foreach (var dayOfMonth in DaysOfMonth)
        {
            switch (dayOfMonth)
            {
                case 1:
                    SafeChangeIsChecked(oneCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 2:
                    SafeChangeIsChecked(twoCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 3:
                    SafeChangeIsChecked(threeCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 4:
                    SafeChangeIsChecked(fourCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 5:
                    SafeChangeIsChecked(fiveCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 6:
                    SafeChangeIsChecked(sixCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 7:
                    SafeChangeIsChecked(sevenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 8:
                    SafeChangeIsChecked(eightCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 9:
                    SafeChangeIsChecked(nineCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 10:
                    SafeChangeIsChecked(tenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 11:
                    SafeChangeIsChecked(elevenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 12:
                    SafeChangeIsChecked(twelveCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 13:
                    SafeChangeIsChecked(thirteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 14:
                    SafeChangeIsChecked(fourteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 15:
                    SafeChangeIsChecked(fifteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 16:
                    SafeChangeIsChecked(sixteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 17:
                    SafeChangeIsChecked(seventeenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 18:
                    SafeChangeIsChecked(eighteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 19:
                    SafeChangeIsChecked(nineteenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 20:
                    SafeChangeIsChecked(twentyCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 21:
                    SafeChangeIsChecked(twentyOneCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 22:
                    SafeChangeIsChecked(twentyTwoCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 23:
                    SafeChangeIsChecked(twentyThreeCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 24:
                    SafeChangeIsChecked(twentyFourCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 25:
                    SafeChangeIsChecked(twentyFiveCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 26:
                    SafeChangeIsChecked(twentySixCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 27:
                    SafeChangeIsChecked(twentySevenCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 28:
                    SafeChangeIsChecked(twentyEightCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 29:
                    SafeChangeIsChecked(twentyNineCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 30:
                    SafeChangeIsChecked(thirtyCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                case 31:
                    SafeChangeIsChecked(thirtyOneCheckBox, dayOfMonth, list.Contains(dayOfMonth));
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}