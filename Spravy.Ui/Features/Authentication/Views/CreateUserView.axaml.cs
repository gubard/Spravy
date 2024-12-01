namespace Spravy.Ui.Features.Authentication.Views;

public partial class CreateUserView : MainUserControl<CreateUserViewModel>
{
    public CreateUserView()
    {
        InitializeComponent();
        DefaultFocusTextBox = EmailTextBox;
    }
}