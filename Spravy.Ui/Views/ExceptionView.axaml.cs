using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

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