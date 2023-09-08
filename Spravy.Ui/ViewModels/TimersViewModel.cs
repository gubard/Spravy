using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Controls;
using Ninject;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class TimersViewModel : RoutableViewModelBase
{
    public TimersViewModel() : base("timers")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        AddTimerCommand = CreateCommandFromTask(AddTimerAsync);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand InitializedCommand { get; }
    public ICommand AddTimerCommand { get; }
    public ICommand SwitchPaneCommand { get; }
    public AvaloniaList<TimerNotify> Timers { get; } = new();

    [Inject]
    public required SplitView SplitView { get; init; }

    [Inject]
    public required IScheduleService ScheduleService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private Task AddTimerAsync()
    {
        return DialogViewer.ShowConfirmDialogAsync<AddTimerView>(
            async _ => DialogViewer.CloseDialog(),
            async view =>
            {
                var parameters = Mapper.Map<AddTimerParameters>(view.ViewModel);
                await ScheduleService.AddTimerAsync(parameters);
                await RefreshAsync();
            }
        );
    }

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
        Timers.AddRange(Mapper.Map<IEnumerable<TimerNotify>>(timers));
    }
}