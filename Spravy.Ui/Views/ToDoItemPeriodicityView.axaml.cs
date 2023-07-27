using System.Windows.Input;
using Avalonia.Markup.Xaml;
using ExtensionFramework.AvaloniaUi.ReactiveUI.Models;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemPeriodicityView  : MainReactiveUserControl<ToDoItemPeriodicityViewModel>,
    IToDoItemView,
    ICompleteSubToDoItemCommand,
    IPathView,
    IChangeCurrentStatusToDoItemCommand
{
    public ToDoItemPeriodicityView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public ICommand CompleteSubToDoItemCommand => ViewModel.ThrowIfNull().CompleteSubToDoItemCommand;
    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
    public ICommand ToRootItemCommand => ViewModel.ThrowIfNull().ToRootItemCommand;
    public ICommand ChangeToDoItemByPathCommand => ViewModel.ThrowIfNull().ChangeToDoItemByPathCommand;
    public ICommand AddSubToDoItemToCurrentCommand => ViewModel.ThrowIfNull().AddSubToDoItemToCurrentCommand;
    public ICommand RemoveSubToDoItemFromCurrentCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromCurrentCommand;
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;
}