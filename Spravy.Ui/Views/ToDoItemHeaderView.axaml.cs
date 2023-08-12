using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemHeaderView : ReactiveUserControl<ToDoItemHeaderViewModel>
{
    public ToDoItemHeaderView()
    {
        InitializeComponent();
    }
}