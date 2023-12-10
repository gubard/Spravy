using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsGroupByView : ReactiveUserControl<ToDoItemsGroupByViewModel>
{
    public ToDoItemsGroupByView()
    {
        InitializeComponent();
    }
}