using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsGroupByStatusView : ReactiveUserControl<ToDoItemsGroupByStatusViewModel>
{
    public ToDoItemsGroupByStatusView()
    {
        InitializeComponent();
    }
}