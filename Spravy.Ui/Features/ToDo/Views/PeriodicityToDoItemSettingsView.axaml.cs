namespace Spravy.Ui.Features.ToDo.Views;

public partial class PeriodicityToDoItemSettingsView : UserControl
{
    public PeriodicityToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not PeriodicityToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not PeriodicityToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
