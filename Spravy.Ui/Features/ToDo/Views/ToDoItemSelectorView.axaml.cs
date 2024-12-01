namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemSelectorView : UserControl
{
    public ToDoItemSelectorView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemSelectorView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemSelectorViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}