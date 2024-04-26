using Avalonia.Controls;

namespace Spravy.Ui.Views;

public partial class MainView : UserControl
{
    public const string ErrorDialogHostName = "error-dialog-host";
    public const string ProgressDialogHostName = "progress-dialog-host";
    public const string InputDialogHostName = "input-dialog-host";
    public const string ContentDialogHostName = "content-dialog-host";
    public const string MainContentName = "main-content-control";
    
    public MainView()
    {
        InitializeComponent();
    }
}