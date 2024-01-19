using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class TodayToDoItemsView : ReactiveUserControl<TodayToDoItemsViewModel>
{
    public TodayToDoItemsView()
    {
        InitializeComponent();
    }
}