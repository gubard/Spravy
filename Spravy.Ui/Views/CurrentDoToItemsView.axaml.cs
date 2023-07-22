using System.Windows.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class CurrentDoToItemsView : ReactiveUserControl<CurrentDoToItemsViewModel>,  
    IToDoItemView,
    ICompleteSubToDoItemCommand,
    IChangeCurrentStatusToDoItemCommand
{
    public CurrentDoToItemsView()
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
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;
    public ICommand AddSubToDoItemToCurrentCommand => ViewModel.ThrowIfNull().AddSubToDoItemToCurrentCommand;
    public ICommand RemoveSubToDoItemFromCurrentCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromCurrentCommand;
}