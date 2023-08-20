using System.Windows.Input;
using Avalonia.Markup.Xaml;
using Spravy.Domain.Extensions;
using Spravy.Ui.Controls;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemGroupView : MainReactiveUserControl<ToDoItemGroupViewModel>, IPathView
{
    public ToDoItemGroupView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public ICommand ToRootItemCommand => ViewModel.ThrowIfNull().ToRootItemCommand;
    public ICommand ChangeToDoItemByPathCommand => ViewModel.ThrowIfNull().ChangeToDoItemByPathCommand;
}