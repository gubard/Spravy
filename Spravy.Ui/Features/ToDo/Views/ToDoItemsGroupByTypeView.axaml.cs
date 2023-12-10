using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsGroupByTypeView : ReactiveUserControl<ToDoItemsGroupByTypeViewModel>
{
    public ToDoItemsGroupByTypeView()
    {
        InitializeComponent();
    }
}