namespace Spravy.Ui.Features.ToDo.Views;

public partial class ValueToDoItemSettingsView : UserControl
{
    public ValueToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ValueToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not ValueToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
