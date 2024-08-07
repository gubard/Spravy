namespace Spravy.Ui.Features.ToDo.Views;

public partial class AddRootToDoItemView : UserControl
{
    public const string ToDoItemContentContentControlName = "to-do-item-content-content-control";

    public AddRootToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not AddRootToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not AddRootToDoItemViewModel viewModel)
            {
                return;
            }

            UiHelper.AddRootToDoItemViewInitialized.Execute(viewModel);
        };
    }
}
