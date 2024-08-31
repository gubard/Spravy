namespace Spravy.Ui.Views;

public partial class MainView : UserControl
{
    private readonly IServiceFactory serviceFactory =
        DiHelper.ServiceFactory.CreateService<IServiceFactory>();

    private readonly IToDoService toDoService =
        DiHelper.ServiceFactory.CreateService<IToDoService>();

    private readonly IUiApplicationService uiApplicationService =
        DiHelper.ServiceFactory.CreateService<IUiApplicationService>();

    public const string MainContentName = "main-content-control";
    public const string MainPanelName = "main-panel-control";

    public MainView()
    {
        InitializeComponent();
        SetPseudoClasses(App.CurrentApp.ThrowIfNull().MaterialDesignSizeType);
        App.MaterialDesignSizeTypeChanged += SetPseudoClasses;
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void SetPseudoClasses(MaterialDesignSizeType sizeType)
    {
        switch (sizeType)
        {
            case MaterialDesignSizeType.ExtraSmall:
                PseudoClasses.Set(":extra-small", true);
                PseudoClasses.Set(":small", false);
                PseudoClasses.Set(":medium", false);
                PseudoClasses.Set(":large", false);
                PseudoClasses.Set(":extra-large", false);
                break;
            case MaterialDesignSizeType.Small:
                PseudoClasses.Set(":extra-small", false);
                PseudoClasses.Set(":small", true);
                PseudoClasses.Set(":medium", false);
                PseudoClasses.Set(":large", false);
                PseudoClasses.Set(":extra-large", false);
                break;
            case MaterialDesignSizeType.Medium:
                PseudoClasses.Set(":extra-small", false);
                PseudoClasses.Set(":small", false);
                PseudoClasses.Set(":medium", true);
                PseudoClasses.Set(":large", false);
                PseudoClasses.Set(":extra-large", false);
                break;
            case MaterialDesignSizeType.Large:
                PseudoClasses.Set(":extra-small", false);
                PseudoClasses.Set(":small", false);
                PseudoClasses.Set(":medium", false);
                PseudoClasses.Set(":large", true);
                PseudoClasses.Set(":extra-large", false);
                break;
            case MaterialDesignSizeType.ExtraLarge:
                PseudoClasses.Set(":extra-small", true);
                PseudoClasses.Set(":small", false);
                PseudoClasses.Set(":medium", false);
                PseudoClasses.Set(":large", false);
                PseudoClasses.Set(":extra-large", true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sizeType), sizeType, null);
        }
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

        var button = control.FindVisualParent<Button>("RootToDoItemButton");

        if (button is null)
        {
            return;
        }

        var pointerPosition = e.GetPosition(button);

        var options =
            pointerPosition.Y > button.Bounds.Height / 2
                ? new UpdateOrderIndexToDoItemOptions(dataItem.Id, sourceItem.Id, true)
                : new(dataItem.Id, sourceItem.Id, false);

        if (UiHelper.DragControl is null)
        {
            return;
        }

        if (UiHelper.DragPanel is null)
        {
            return;
        }

        UiHelper.DragPanel.Children.Remove(UiHelper.DragControl);
        await toDoService.UpdateToDoItemOrderIndexAsync(options, CancellationToken.None);
        await uiApplicationService.RefreshCurrentViewAsync(CancellationToken.None);
    }
}
