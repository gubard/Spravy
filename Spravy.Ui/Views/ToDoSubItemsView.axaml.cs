using Spravy.Ui.Controls;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoSubItemsView : MainReactiveUserControl<ToDoSubItemsViewModel>
{
    public const string ListContentControlName = "list-content-control";

    public ToDoSubItemsView()
    {
        InitializeComponent();
    }
}