using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class PageHeaderViewModel : ViewModelBase
{
    private ToDoItemCommand? leftCommand;
    private ToDoItemCommand? rightCommand;
    private object? content;

    public PageHeaderViewModel()
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ToDoItemCommand? LeftCommand
    {
        get => leftCommand;
        set => this.RaiseAndSetIfChanged(ref leftCommand, value);
    }

    public ToDoItemCommand? RightCommand
    {
        get => rightCommand;
        set => this.RaiseAndSetIfChanged(ref rightCommand, value);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public AvaloniaList<ToDoItemCommand> Commands { get; } = new();
    public PageHeaderView? ToDoItemHeaderView { get; set; }
    public ICommand SwitchPaneCommand { get; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    private DispatcherOperation SwitchPane()
    {
        return this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }
}