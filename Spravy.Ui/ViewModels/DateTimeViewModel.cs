using System;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DateTimeViewModel : ViewModelBase
{
    private DateTime? selectedDate;
    private TimeSpan? selectedTime;

    public DateTime? SelectedDate
    {
        get => selectedDate;
        set => this.RaiseAndSetIfChanged(ref selectedDate, value);
    }

    public TimeSpan? SelectedTime
    {
        get => selectedTime;
        set => this.RaiseAndSetIfChanged(ref selectedTime, value);
    }
}