using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ErrorHandling.ViewModels;

namespace Spravy.Ui.Features.ErrorHandling.Views;

public partial class ExceptionView : ReactiveUserControl<ExceptionViewModel>
{
    public ExceptionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}