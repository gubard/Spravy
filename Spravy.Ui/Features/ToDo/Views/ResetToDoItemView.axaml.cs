namespace Spravy.Ui.Features.ToDo.Views;

public partial class ResetToDoItemView : UserControl
{
    public const string IsMoveCircleOrderIndexCheckBoxName = "is-move-circle-order-index-check-box";

    public ResetToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ResetToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not ResetToDoItemViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
