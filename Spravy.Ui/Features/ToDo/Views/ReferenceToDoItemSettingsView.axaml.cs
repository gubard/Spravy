namespace Spravy.Ui.Features.ToDo.Views;

public partial class ReferenceToDoItemSettingsView : UserControl
{
    public ReferenceToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ReferenceToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not ReferenceToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
