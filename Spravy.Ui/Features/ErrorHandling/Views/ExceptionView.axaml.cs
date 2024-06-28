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
