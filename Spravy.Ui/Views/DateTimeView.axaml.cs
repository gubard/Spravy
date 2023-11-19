using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DateTimeView : ReactiveUserControl<DateTimeViewModel>
{
    public DateTimeView()
    {
        InitializeComponent();
    }
}