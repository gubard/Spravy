namespace Spravy.Ui.Features.ToDo.Views;

public partial class PeriodicityOffsetToDoItemSettingsView : UserControl
{
    public PeriodicityOffsetToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not PeriodicityOffsetToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not PeriodicityOffsetToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
