namespace Spravy.Ui.Models;

public class TaskProgressItem : NotifyBase
{
    public TaskProgressItem(ushort impact)
    {
        Impact = impact;
    }

    public ushort Impact { get; }

    [Reactive]
    public ushort Progress { get; set; }

    public bool IsFinished
    {
        get => Progress >= Impact;
    }

    public void Finish()
    {
        Progress = Impact;
    }

    public ConfiguredValueTaskAwaitable<Result> IncreaseAsync()
    {
        return this.InvokeUiBackgroundAsync(() =>
        {
            Progress++;

            return Result.Success;
        });
    }
}
