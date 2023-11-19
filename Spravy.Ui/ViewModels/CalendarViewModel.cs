using System;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class CalendarViewModel : ViewModelBase
{
    private DateTime? selectedDate;

    public DateTime? SelectedDate
    {
        get => selectedDate;
        set => this.RaiseAndSetIfChanged(ref selectedDate, value);
    }
}