namespace Spravy.Ui.Features.ToDo.Views;

public partial class AddRootToDoItemView : ReactiveUserControl<AddRootToDoItemViewModel>
{
    public const string ToDoItemContentContentControlName = "to-do-item-content-content-control";

    public AddRootToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
