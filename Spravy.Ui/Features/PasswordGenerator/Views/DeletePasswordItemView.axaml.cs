namespace Spravy.Ui.Features.PasswordGenerator.Views;

public partial class DeletePasswordItemView : UserControl
{
    public DeletePasswordItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not DeletePasswordItemView view)
            {
                return;
            }

            if (view.DataContext is not DeletePasswordItemViewModel viewModel)
            {
                return;
            }

            UiHelper.DeletePasswordItemViewInitialized.Execute(viewModel);
        };
    }
}
