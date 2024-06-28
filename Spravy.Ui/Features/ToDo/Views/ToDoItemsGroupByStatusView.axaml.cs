namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemsGroupByStatusView
    : ReactiveUserControl<ToDoItemsGroupByStatusViewModel>
{
    public const string ReadyForCompletedContentControlName = "ready-for-completed-content-control";

    public ToDoItemsGroupByStatusView()
    {
        InitializeComponent();
    }
}
