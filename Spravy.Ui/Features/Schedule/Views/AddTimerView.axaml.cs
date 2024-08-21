namespace Spravy.Ui.Features.Schedule.Views;

public partial class AddTimerView : MainUserControl<AddTimerViewModel>
{
    public AddTimerView()
    {
        InitializeComponent();

        Initialized += (s, _) =>
        {
            if (s is not AddTimerView view)
            {
                return;
            }

            if (view.DataContext is not AddTimerViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
