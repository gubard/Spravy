namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemSettingsView : UserControl
{
    public ToDoItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemSettingsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
