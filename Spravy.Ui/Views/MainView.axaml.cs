using Avalonia.Input;

namespace Spravy.Ui.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    private readonly IServiceFactory serviceFactory;
    private readonly IToDoService toDoService;
    private readonly IUiApplicationService uiApplicationService;
    public const string ErrorDialogHostName = "error-dialog-host";
    public const string ProgressDialogHostName = "progress-dialog-host";
    public const string InputDialogHostName = "input-dialog-host";
    public const string ContentDialogHostName = "content-dialog-host";
    public const string MainContentName = "main-content-control";
    public const string MainPanelName = "main-panel-control";

    public MainView(
        IServiceFactory serviceFactory,
        IToDoService toDoService,
        IUiApplicationService uiApplicationService
    )
    {
        this.serviceFactory = serviceFactory;
        this.toDoService = toDoService;
        this.uiApplicationService = uiApplicationService;
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);

        this.AddAdaptiveStyle(
            new[] { MaterialDesignSizeType.ExtraSmall, MaterialDesignSizeType.Small, },
            "AdaptiveCommandsSmall"
        );

        this.AddAdaptiveStyle(
            new[]
            {
                MaterialDesignSizeType.Medium,
                MaterialDesignSizeType.Large,
                MaterialDesignSizeType.ExtraLarge,
            },
            "AdaptiveCommandsWide"
        );

        this.AddAdaptiveStyle(
            new[]
            {
                MaterialDesignSizeType.ExtraSmall,
                MaterialDesignSizeType.Small,
                MaterialDesignSizeType.Medium,
            },
            "ToDoItemsGroupSmall"
        );

        this.AddAdaptiveStyle(
            new[] { MaterialDesignSizeType.Large, MaterialDesignSizeType.ExtraLarge, },
            "ToDoItemsGroupWide"
        );

        this.AddAdaptiveStyle(new[] { MaterialDesignSizeType.ExtraSmall, }, "DialogHostExtraSmall");
        this.AddAdaptiveStyle(new[] { MaterialDesignSizeType.Small, }, "DialogHostSmall");
        this.AddAdaptiveStyle(new[] { MaterialDesignSizeType.Medium, }, "DialogHostMedium");

        this.AddAdaptiveStyle(
            new[] { MaterialDesignSizeType.Large, MaterialDesignSizeType.ExtraLarge },
            "DialogHostLarge"
        );
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

    private async void Drop(object? sender, DragEventArgs e)
    {
        var data = e.Data.Get(UiHelper.ToDoItemEntityNotifyDataFormat);

        if (data is not ToDoItemEntityNotify dataItem)
        {
            return;
        }

        if (e.Source is not Control control)
        {
            return;
        }

        if (control.DataContext is not ToDoItemEntityNotify sourceItem)
        {
            return;
        }

        if (dataItem.Equals(sourceItem))
        {
            return;
        }

        var button = control.FindVisualParent<Button>(App.RootToDoItemButtonName);

        if (button is null)
        {
            return;
        }

        var pointerPosition = e.GetPosition(button);

        if (pointerPosition.Y > button.Bounds.Height / 2)
        {
            var options = new UpdateOrderIndexToDoItemOptions(dataItem.Id, sourceItem.Id, true);
            await toDoService.UpdateToDoItemOrderIndexAsync(options, CancellationToken.None);
        }
        else
        {
            var options = new UpdateOrderIndexToDoItemOptions(dataItem.Id, sourceItem.Id, false);
            await toDoService.UpdateToDoItemOrderIndexAsync(options, CancellationToken.None);
        }

        await uiApplicationService.RefreshCurrentViewAsync(CancellationToken.None);
    }
}
