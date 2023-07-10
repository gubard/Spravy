using System.Windows.Input;
using Avalonia.Markup.Xaml;
using ExtensionFramework.AvaloniaUi.ReactiveUI.Models;
using ExtensionFramework.Core.Common.Extensions;
using Spravy.Interfaces;
using Spravy.ViewModels;

namespace Spravy.Views;

public partial class ToDoItemView : MainReactiveUserControl<ToDoItemViewModel>, IToDoItemView
{
    public ToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public ICommand DeleteSubToDoItemCommand => ViewModel.ThrowIfNull().DeleteSubToDoItemCommand;
    public ICommand ChangeToDoItemCommand => ViewModel.ThrowIfNull().ChangeToDoItemCommand;
}