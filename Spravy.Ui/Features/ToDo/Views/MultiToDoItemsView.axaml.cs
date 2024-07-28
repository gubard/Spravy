using Avalonia.Input;
using Spravy.Core.Helpers;

namespace Spravy.Ui.Features.ToDo.Views;

public partial class MultiToDoItemsView : ReactiveUserControl<MultiToDoItemsViewModel>
{
    public const string ContentContentControlName = "content-content-control";

    public MultiToDoItemsView()
    {
        InitializeComponent();
    }

    private async void ReorderOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var mainPanel = DiHelper
            .ServiceFactory.CreateService<MainView>()
            .FindControl<Panel>(MainView.MainPanelName);

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
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
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
