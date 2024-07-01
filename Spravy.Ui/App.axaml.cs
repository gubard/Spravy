using Avalonia.Input;
using Avalonia.Layout;
using Spravy.Core.Helpers;
using Spravy.Ui.Features.ToDo.Views;

namespace Spravy.Ui;

public class App : Application
{
    private readonly IServiceFactory serviceFactory = DiHelper.ServiceFactory;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = serviceFactory.CreateService<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var objectStorage = serviceFactory.CreateService<IObjectStorage>();
        var ct = CancellationToken.None;

        if (
            objectStorage
                .IsExistsAsync(TypeCache<SettingModel>.Type.Name, ct)
                .GetAwaiter()
                .GetResult()
                .Value
        )
        {
            var model = objectStorage
                .GetObjectAsync<SettingModel>(TypeCache<SettingModel>.Type.Name, ct)
                .GetAwaiter()
                .GetResult()
                .Value;

            var theme = SukiTheme.GetInstance();
            theme.ChangeColorTheme(
                theme.ColorThemes.Single(x => x.DisplayName == model.ColorTheme)
            );

            theme.ChangeBaseTheme(
                model.BaseTheme switch
                {
                    "Light" => ThemeVariant.Light,
                    "Dark" => ThemeVariant.Dark,
                    _ => throw new ArgumentOutOfRangeException(),
                }
            );
        }

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
            var control = serviceFactory.CreateService<ISingleViewTopLevelControl>().As<Control>();

            singleViewPlatform.MainView = control;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void ReorderOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Current is null)
        {
            return;
        }

        var mainPanel = serviceFactory
            .CreateService<MainView>()
            .FindControl<Panel>(MainView.MainPanelName);

        var topLevel = serviceFactory.CreateService<TopLevel>();

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

        try
        {
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
        finally
        {
            UiHelper.IsDrag = false;
        }

        mainPanel.Children.Remove(contentControl);
    }
}
