namespace Spravy.Ui.Views;

public partial class ConfirmView : DialogableUserControl<ConfirmViewModel>
{
    public const string ContentContentControlName = "content-content-control";
    public const string OkButtonName = "ok-button";
    public const string CancelButtonName = "cancel-button";

    public ConfirmView()
    {
        InitializeComponent();
    }
}
