using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class PageHeaderView : ReactiveUserControl<PageHeaderViewModel>
{
    public PageHeaderView()
    {
        InitializeComponent();
    }
}