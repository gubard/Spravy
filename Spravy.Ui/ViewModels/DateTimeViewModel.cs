using System;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DateTimeViewModel : ViewModelBase
{
    [Reactive]
    public DateTime? SelectedDate { get; set; }

    [Reactive]
    public TimeSpan? SelectedTime { get; set; }
}