using System.Windows.Input;
using Spravy.Domain.Extensions;
using Spravy.Ui.Controls;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoSubItemsView : MainReactiveUserControl<ToDoSubItemsViewModel>,
    IToDoItemView,
    ICompleteSubToDoItemCommand,
    IChangeFavoriteToDoItemCommand
{
    public ToDoSubItemsView()
    {
        InitializeComponent();
    }

    public ICommand CompleteSubToDoItemCommand => ViewModel.ThrowIfNull().CompleteSubToDoItemCommand;
    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
    public ICommand AddSubToDoItemToFavoriteCommand => ViewModel.ThrowIfNull().AddSubToDoItemToFavoriteCommand;
    public ICommand RemoveSubToDoItemFromFavoriteCommand => ViewModel.ThrowIfNull().RemoveSubToDoItemFromFavoriteCommand;
    public ICommand ChangeToActiveDoItemCommand => ViewModel.ThrowIfNull().ChangeToActiveDoItemCommand;
    public ICommand ChangeOrderIndexCommand => ViewModel.ThrowIfNull().ChangeOrderIndexCommand;
}