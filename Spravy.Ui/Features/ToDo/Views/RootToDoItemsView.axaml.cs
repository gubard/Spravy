namespace Spravy.Ui.Features.ToDo.Views;

public partial class RootToDoItemsView : UserControl
{
    public const string AddRootToDoItemButtonName = "add-root-to-do-item-button";
    public const string ToDoSubItemsContentControlName = "to-do-sub-items-content-control";

    public RootToDoItemsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
