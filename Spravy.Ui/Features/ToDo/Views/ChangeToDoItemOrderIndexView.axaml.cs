namespace Spravy.Ui.Features.ToDo.Views;

public partial class ChangeToDoItemOrderIndexView : UserControl
{
    public const string ItemsListBoxName = "items-list-box";

    public ChangeToDoItemOrderIndexView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ChangeToDoItemOrderIndexView view)
            {
                return;
            }

            if (view.DataContext is not ChangeToDoItemOrderIndexViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
