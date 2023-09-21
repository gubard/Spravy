using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Controls;
using Ninject;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class TimersViewModel : RoutableViewModelBase
{
    public TimersViewModel() : base("timers")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand InitializedCommand { get; }
    public ICommand SwitchPaneCommand { get; }
    public AvaloniaList<TimerItemNotify> Timers { get; } = new();

    [Inject]
    public required SplitView SplitView { get; init; }

    [Inject]
    public required IScheduleService ScheduleService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }

    private Task InitializedAsync()
    {
        return RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var timers = await ScheduleService.GetListTimesAsync();
        Timers.Clear();
        Timers.AddRange(Mapper.Map<IEnumerable<TimerItemNotify>>(timers));
    }
}