using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Spravy.Domain.Extensions;
using Spravy.Ui.Controls;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoSubItemsView : MainReactiveUserControl<ToDoSubItemsViewModel>,
    IToDoItemView,
    ICompleteSubToDoItemCommand,
    IChangePinnedToDoItemCommand
{
    private ScrollViewer? mainScrollViewer;
    private Button? scrollUpButton;
    private Button? scrollButtonBottom;

    public ToDoSubItemsView()
    {
        InitializeComponent();
    }

    public ICommand CompleteSubToDoItemCommand => ViewModel.ThrowIfNull().CompleteSubToDoItemCommand;
    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
    public ICommand AddSubToDoItemToPinnedCommand => ViewModel.ThrowIfNull().AddSubToDoItemToPinnedCommand;
    public ICommand RemoveSubToDoItemFromPinnedCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromPinnedCommand;
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;
    public ICommand UpOrderIndexCommand=> ViewModel.ThrowIfNull().UpOrderIndexCommand;
    public ICommand DownOrderIndexCommand => ViewModel.ThrowIfNull().DownOrderIndexCommand;
}