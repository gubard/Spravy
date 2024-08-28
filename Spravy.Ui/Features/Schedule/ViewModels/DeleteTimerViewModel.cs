namespace Spravy.Ui.Features.Schedule.ViewModels;

public class DeleteTimerViewModel : DialogableViewModelBase
{
    public DeleteTimerViewModel(TimerItemNotify item)
    {
        Item = item;
    }

    public TimerItemNotify Item { get; }

    public override string ViewId
    {
        get => $"{TypeCache<DeleteTimerViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
