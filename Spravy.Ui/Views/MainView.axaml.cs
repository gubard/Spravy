using Avalonia.Input;

namespace Spravy.Ui.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    private readonly IServiceFactory serviceFactory;
    public const string ErrorDialogHostName = "error-dialog-host";
    public const string ProgressDialogHostName = "progress-dialog-host";
    public const string InputDialogHostName = "input-dialog-host";
    public const string ContentDialogHostName = "content-dialog-host";
    public const string MainContentName = "main-content-control";
    public const string MainPanelName = "main-panel-control";

    public MainView(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!UiHelper.IsDrag)
        {
            return;
        }

        var mainPanel = this.FindControl<Panel>(MainPanelName);

        if (mainPanel is null)
        {
            return;
        }

        var control = mainPanel.Children.LastOrDefault();

        if (control is null)
        {
            return;
        }

        var topLevel = serviceFactory.CreateService<TopLevel>();
        var mousePosition = e.GetPosition(topLevel);
        var x = mousePosition.X + 20;
        var y = mousePosition.Y - control.Bounds.Height / 2 + 20;
        control.RenderTransform = new TranslateTransform(x, y);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var data = e.Data.Get(UiHelper.ToDoItemEntityNotifyDataFormat);

        if (data is not ToDoItemEntityNotify taskItem)
        {
            return;
        }
    }
}
