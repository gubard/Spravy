using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemView : ReactiveUserControl<ToDoItemViewModel>
{
    public ToDoItemView()
    {
        InitializeComponent();
    }
}