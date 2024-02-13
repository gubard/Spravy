using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemDayOfWeekSelectorView : ReactiveUserControl<ToDoItemDayOfWeekSelectorViewModel>
{
    public ToDoItemDayOfWeekSelectorView()
    {
        InitializeComponent();
    }
}