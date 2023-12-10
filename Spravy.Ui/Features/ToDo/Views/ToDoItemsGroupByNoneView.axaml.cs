using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsGroupByNoneView : ReactiveUserControl<ToDoItemsGroupByNoneViewModel>
{
    public ToDoItemsGroupByNoneView()
    {
        InitializeComponent();
    }
}