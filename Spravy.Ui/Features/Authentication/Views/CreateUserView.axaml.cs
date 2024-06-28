namespace Spravy.Ui.Features.Authentication.Views;

public partial class CreateUserView : ReactiveUserControl<CreateUserViewModel>
{
    public const string EmailTextBoxName = "email-text-box";
    public const string LoginTextBoxName = "login-text-box";
    public const string PasswordTextBoxName = "password-text-box";
    public const string RepeatPasswordTextBoxName = "repeat-password-text-box";
    public const string CreateUserCardName = "create-user-card";
    public const string BackButtonName = "back-button";
    public const string CreateUserButtonName = "create-user-button";

    public CreateUserView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FindControl<TextBox>(EmailTextBoxName)?.Focus();
    }
}
