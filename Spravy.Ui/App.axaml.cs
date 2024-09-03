using Avalonia.Layout;

namespace Spravy.Ui;

public class App : Application
{
    private static MaterialDesignSizeType materialDesignSizeType;

    public static App? CurrentApp
    {
        get => (App?)Current;
    }

    private readonly IServiceFactory serviceFactory = DiHelper.ServiceFactory;

    public static readonly DirectProperty<
        App,
        MaterialDesignSizeType
    > MaterialDesignSizeTypeProperty = AvaloniaProperty.RegisterDirect<App, MaterialDesignSizeType>(
        nameof(MaterialDesignSizeType),
        app => app.MaterialDesignSizeType
    );

    public static event Action<MaterialDesignSizeType>? MaterialDesignSizeTypeChanged;

    static App()
    {
        Layoutable.WidthProperty.Changed.AddClassHandler<TopLevel>(
            (topLevel, _) =>
            {
                var currentType = topLevel.Width switch
                {
                    <= MaterialDesign.MaxExtraSmall => MaterialDesignSizeType.ExtraSmall,
                    > MaterialDesign.MaxExtraSmall
                    and <= MaterialDesign.MaxSmall
                        => MaterialDesignSizeType.Small,
                    > MaterialDesign.MaxSmall
                    and <= MaterialDesign.MaxMedium
                        => MaterialDesignSizeType.Medium,
                    > MaterialDesign.MaxMedium
                    and <= MaterialDesign.MaxLarge
                        => MaterialDesignSizeType.Large,
                    > MaterialDesign.MaxLarge => MaterialDesignSizeType.ExtraLarge,
                    _ => MaterialDesignSizeType.ExtraSmall,
                };

                if (CurrentApp is null)
                {
                    return;
                }

                CurrentApp.MaterialDesignSizeType = currentType;
                MaterialDesignSizeTypeChanged?.Invoke(currentType);
            }
        );
    }

    public MaterialDesignSizeType MaterialDesignSizeType
    {
        get => materialDesignSizeType;
        private set =>
            SetAndRaise(MaterialDesignSizeTypeProperty, ref materialDesignSizeType, value);
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = serviceFactory.CreateService<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = serviceFactory
                .CreateService<IDesktopTopLevelControl>()
                .As<Window>()
                .ThrowIfNull();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = serviceFactory
                .CreateService<IFactory<ISingleViewTopLevelControl>>()
                .Create()
                .ThrowIfError()
                .As<Control>();

            singleViewPlatform.MainView = control;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void ToDoItemTapped(object? sender, TappedEventArgs e)
    {
        if (!OperatingSystem.IsAndroid())
        {
            return;
        }

        if (!UiHelper.IsDrag)
        {
            return;
        }

        var data = UiHelper.DragData;

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

        e.Handled = true;
        UiHelper.IsDrag = false;
        var pointerPosition = e.GetPosition(button);
        var options =
            pointerPosition.Y > button.Bounds.Height / 2
                ? new UpdateOrderIndexToDoItemOptions(dataItem.Id, sourceItem.Id, true)
                : new(dataItem.Id, sourceItem.Id, false);
        await serviceFactory
            .CreateService<IToDoService>()
            .UpdateToDoItemOrderIndexAsync(options, CancellationToken.None);
        await serviceFactory
            .CreateService<IUiApplicationService>()
            .RefreshCurrentViewAsync(CancellationToken.None);
    }

    private async void ReorderOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var mainPanel = DiHelper
            .ServiceFactory.CreateService<MainView>()
            .FindControl<Panel>("MainPanel");

        var topLevel = DiHelper.ServiceFactory.CreateService<TopLevel>();

        if (mainPanel is null)
        {
            return;
        }

        if (sender is not Visual visual)
        {
            return;
        }

        if (visual.DataContext is not ToDoItemEntityNotify toDoItem)
        {
            return;
        }

        var button = visual.FindVisualParent<Button>();

        if (button is null)
        {
            return;
        }

        var contentControl = new ContentControl
        {
            Width = button.Bounds.Width,
            Height = button.Bounds.Height,
            Content = toDoItem,
            ZIndex = 100,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
        };

        var mousePosition = e.GetPosition(topLevel);
        mainPanel.Children.Add(contentControl);
        var x = mousePosition.X + 20;
        var y = mousePosition.Y - button.Bounds.Height / 2 + 20;
        contentControl.RenderTransform = new TranslateTransform(x, y);
        var dragData = new DataObject();
        dragData.Set(UiHelper.ToDoItemEntityNotifyDataFormat, toDoItem);
        UiHelper.IsDrag = true;
        UiHelper.DragControl = contentControl;
        UiHelper.DragPanel = mainPanel;

        try
        {
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
        finally
        {
            UiHelper.DragData = toDoItem;
            UiHelper.IsDrag = false;
        }

        mainPanel.Children.Remove(contentControl);
    }
}
