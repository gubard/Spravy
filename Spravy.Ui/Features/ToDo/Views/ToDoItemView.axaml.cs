namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemView : NavigatableUserControl<ToDoItemViewModel>
{
    public ToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemViewModel viewModel)
            {
                return;
            }

            viewModel.Commands.InitializedCommand.Command.Execute(viewModel);
        };
    }
}
