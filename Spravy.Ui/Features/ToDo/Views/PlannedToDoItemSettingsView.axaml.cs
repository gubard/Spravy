namespace Spravy.Ui.Features.ToDo.Views;

public partial class PlannedToDoItemSettingsView : UserControl
{
    public PlannedToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not PlannedToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not PlannedToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
