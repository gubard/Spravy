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
        this.PostUiBackground(() =>
        {
            Progress = Impact;
        
            return Result.Success;
        }, CancellationToken.None);
    }

    public Result IncreaseUi()
    {
        Progress++;

        return Result.Success;
    }
}
