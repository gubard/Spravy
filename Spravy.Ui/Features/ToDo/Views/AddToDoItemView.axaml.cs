namespace Spravy.Ui.Features.ToDo.Views;

public partial class AddToDoItemView : UserControl
{
    public AddToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not AddToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not AddToDoItemViewModel viewModel)
            {
                return;
            }

            UiHelper.AddToDoItemViewInitialized.Execute(viewModel);
        };
    }
}
