using Avalonia.Input;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsView : ReactiveUserControl<ToDoItemsViewModel>
{
    public const string PanelName = "panel-control";
    public const string ItemsItemsControlName = "items-items-control";

    public const string ItemsItemsControlVirtualizingStackPanelName =
        "items-items-control-virtualizing-stack-panel";

    public ToDoItemsView()
    {
        InitializeComponent();
    }
}
