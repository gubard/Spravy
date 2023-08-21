using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class CreateUserView : ReactiveUserControl<CreateUserViewModel>
{
    public const string LoginTextBoxName = "LoginTextBox";
    public const string PasswordTextBoxName = "PasswordTextBox";
    public const string RepeatPasswordTextBoxName = "RepeatPasswordTextBox";
    
    public CreateUserView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FindControl<TextBox>(LoginTextBoxName)?.Focus();
    }
}