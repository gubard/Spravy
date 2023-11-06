using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Spravy.Domain.Extensions;
using Spravy.Ui.Controls;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemCircleView : MainReactiveUserControl<ToDoItemCircleViewModel>, IPathView
{
    public ToDoItemCircleView()
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