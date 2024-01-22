using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class AccountSettingView : ReactiveUserControl<AccountSettingViewModel>
{
    public AccountSettingView()
    {
        InitializeComponent();
    }
}