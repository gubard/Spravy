using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemHeaderView : ReactiveUserControl<ToDoItemHeaderViewModel>
{
    public ToDoItemHeaderView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var t = this.FindControl<ItemsControl>("Asdasdasdasdasd");
    }
}