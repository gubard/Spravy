namespace Spravy.Ui.Features.PasswordGenerator.Views;

public partial class PasswordGeneratorView : UserControl
{
    public PasswordGeneratorView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not PasswordGeneratorView view)
            {
                return;
            }

            if (view.DataContext is not PasswordGeneratorViewModel viewModel)
            {
                return;
            }

            UiHelper.PasswordGeneratorViewInitialized.Execute(viewModel);
        };
    }
}
