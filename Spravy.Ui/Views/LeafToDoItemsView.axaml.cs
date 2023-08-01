using System.Windows.Input;
using Avalonia.Markup.Xaml;
using ExtensionFramework.AvaloniaUi.ReactiveUI.Models;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class LeafToDoItemsView : ReactiveUserControl<LeafToDoItemsViewModel>, IToDoItemView , ICompleteSubToDoItemCommand
{
    public LeafToDoItemsView()
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
}