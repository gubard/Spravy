using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;
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
        ViewModel.ThrowIfNull().ItemsControlCommands = this.FindControl<ItemsControl>("ItemsControlCommands");
    }
}