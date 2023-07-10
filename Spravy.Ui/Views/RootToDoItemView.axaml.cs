using System.Windows.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class RootToDoItemView : ReactiveUserControl<RootToDoItemViewModel>, IToDoItemView
{
    public RootToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public ICommand SkipSubToDoItemCommand => ViewModel.ThrowIfNull().SkipSubToDoItemCommand;
    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
}