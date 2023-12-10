using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsView : ReactiveUserControl<ToDoItemsViewModel>
{
    public ToDoItemsView()
    {
        InitializeComponent();
    }
}