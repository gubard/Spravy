using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class PageHeaderView : ReactiveUserControl<PageHeaderViewModel>
{
    public PageHeaderView()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ViewModel.ThrowIfNull().ToDoItemHeaderView = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ViewModel.ThrowIfNull().ToDoItemHeaderView = this;
    }
}