using System.Windows.Input;
using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemView : ReactiveUserControl<ToDoItemViewModel>, IPathView
{
    public ToDoItemView()
    {
        InitializeComponent();
    }

    public ICommand ToRootItemCommand => ViewModel.ThrowIfNull().ToRootItemCommand;
    public ICommand ChangeToDoItemByPathCommand => ViewModel.ThrowIfNull().ChangeToDoItemByPathCommand;
}