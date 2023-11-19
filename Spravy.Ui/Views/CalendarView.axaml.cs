using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class CalendarView : ReactiveUserControl<CalendarViewModel>
{
    public CalendarView()
    {
        InitializeComponent();
    }
}