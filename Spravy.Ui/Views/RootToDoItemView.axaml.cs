using System.Windows.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class RootToDoItemView : ReactiveUserControl<RootToDoItemViewModel>, IToDoItemView, ICompleteSubToDoItemCommand
{
    public RootToDoItemView()
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
    public ICommand ChangeParentToDoItemCommand  => ViewModel.ThrowIfNull().ChangeParentToDoItemCommand;
}