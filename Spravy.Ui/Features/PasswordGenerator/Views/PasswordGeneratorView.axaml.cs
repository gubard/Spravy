using Avalonia.ReactiveUI;
using Spravy.Ui.Features.PasswordGenerator.ViewModels;

namespace Spravy.Ui.Features.PasswordGenerator.Views;

public partial class PasswordGeneratorView : ReactiveUserControl<PasswordGeneratorViewModel>
{
    public PasswordGeneratorView()
    {
        InitializeComponent();
    }
}