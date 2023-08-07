using System.Windows.Input;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoSubItemsView : ReactiveUserControl<ToDoSubItemsViewModel>,
    IToDoItemView,
    ICompleteSubToDoItemCommand,
    IChangeCurrentStatusToDoItemCommand
{
    public ToDoSubItemsView()
    {
        InitializeComponent();
    }

    public ICommand CompleteSubToDoItemCommand => ViewModel.ThrowIfNull().CompleteSubToDoItemCommand;
    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
    public ICommand AddSubToDoItemToCurrentCommand => ViewModel.ThrowIfNull().AddSubToDoItemToCurrentCommand;
    public ICommand RemoveSubToDoItemFromCurrentCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromCurrentCommand;
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;
}