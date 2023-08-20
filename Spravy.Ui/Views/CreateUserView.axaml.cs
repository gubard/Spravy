using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class CreateUserView : ReactiveUserControl<CreateUserViewModel>
{
    public CreateUserView()
    {
        InitializeComponent();
    }
}