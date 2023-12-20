using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class TimersViewModel : NavigatableViewModelBase
{
    public TimersViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public AvaloniaList<TimerItemNotify> Timers { get; } = new();

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    [Inject]
    public required IScheduleService ScheduleService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var timers = await ScheduleService.GetListTimesAsync(cancellationToken).ConfigureAwait(false);
        Timers.Clear();
        Timers.AddRange(Mapper.Map<IEnumerable<TimerItemNotify>>(timers));
    }

    public override void Stop()
    {
        
    }
}