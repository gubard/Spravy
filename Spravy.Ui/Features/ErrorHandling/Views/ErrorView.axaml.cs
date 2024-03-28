using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ErrorHandling.ViewModels;

namespace Spravy.Ui.Features.ErrorHandling.Views;

public partial class ErrorView : ReactiveUserControl<ErrorViewModel>
{
    public ErrorView()
    {
        InitializeComponent();
    }
}