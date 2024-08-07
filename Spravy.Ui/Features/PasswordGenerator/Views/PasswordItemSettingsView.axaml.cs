namespace Spravy.Ui.Features.PasswordGenerator.Views;

public partial class PasswordItemSettingsView : UserControl
{
    public PasswordItemSettingsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not PasswordItemSettingsView view)
            {
                return;
            }

            if (view.DataContext is not PasswordItemSettingsViewModel viewModel)
            {
                return;
            }

            UiHelper.PasswordItemSettingsViewInitialized.Execute(viewModel);
        };
    }
}
