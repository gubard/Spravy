using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemDayOfMonthSelectorView : ReactiveUserControl<ToDoItemDayOfMonthSelectorViewModel>
{
    public ToDoItemDayOfMonthSelectorView()
    {
        InitializeComponent();
    }
}