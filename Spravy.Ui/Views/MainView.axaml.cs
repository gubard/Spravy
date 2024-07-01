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

        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
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

        control.RenderTransform = new TranslateTransform(
            mousePosition.X - control.Bounds.Width / 2,
            mousePosition.Y - control.Bounds.Height / 2 + 20
        );

        e.DragEffects = DragDropEffects.Move;

        var data = e.Data.Get("to-do-item");

        if (data is not ToDoItemEntityNotify taskItem)
            return;

        /*if (!vm.IsDestinationValid(taskItem, (e.Source as Control)?.Name))
        {
            e.DragEffects = DragDropEffects.None;
        }*/
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        Console.WriteLine("Drop");

        var data = e.Data.Get("to-do-item");

        if (data is not ToDoItemEntityNotify taskItem)
        {
            return;
        }
    }
}
