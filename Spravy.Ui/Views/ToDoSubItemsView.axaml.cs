using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
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
    public const string MainScrollViewerName = "MainScrollViewer";
    public const string ScrollUpButtonName = "ScrollUpButton";
    public const string ScrollButtonBottomName = "ScrollButtonBottom";

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
    public ICommand AddSubToDoItemToCurrentCommand => ViewModel.ThrowIfNull().AddSubToDoItemToCurrentCommand;
    public ICommand RemoveSubToDoItemFromCurrentCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromCurrentCommand;
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        mainScrollViewer = this.FindControl<ScrollViewer>(MainScrollViewerName).ThrowIfNull();
        SetupButton(ref scrollUpButton, ScrollUpButtonName, ScrollUp);
        SetupButton(ref scrollButtonBottom, ScrollButtonBottomName, ScrollBottom);
    }

    private void SetupButton(ref Button? button, string name, EventHandler<RoutedEventArgs> handler)
    {
        if (button is not null)
        {
            button.Click -= handler;
        }

        button = this.FindControl<Button>(name).ThrowIfNull();
        button.Click += handler;
    }

    private void ScrollUp(object? sender, RoutedEventArgs e)
    {
        mainScrollViewer?.ScrollToHome();
    }

    private void ScrollBottom(object? sender, RoutedEventArgs e)
    {
        mainScrollViewer?.ScrollToEnd();
    }
}