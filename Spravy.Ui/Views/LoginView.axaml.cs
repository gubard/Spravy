using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public const string LoginTextBoxName = "LoginTextBox";
    public const string PasswordTextBoxName = "PasswordTextBox";
    
    public LoginView()
    {
        InitializeComponent();
    }
}