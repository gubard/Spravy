namespace Spravy.Ui.Models;

public partial class TaskProgressItem : NotifyBase
{
    [ObservableProperty]
    private ushort progress;

    public TaskProgressItem(ushort impact)
    {
        Impact = impact;
    }

    public ushort Impact { get; }

    public bool IsFinished
    {
        get => Progress >= Impact;
    }

    public void Finish()
    {
        this.PostUiBackground(
            () =>
            {
                Progress = Impact;

                return Result.Success;
            },
            CancellationToken.None
        );
    }

    public Result IncreaseUi()
    {
        Progress++;

        return Result.Success;
    }
}
