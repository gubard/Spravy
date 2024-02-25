using System;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class CalendarViewModel : ViewModelBase
{
    [Reactive]
    public DateTime? SelectedDate { get; set; }
}