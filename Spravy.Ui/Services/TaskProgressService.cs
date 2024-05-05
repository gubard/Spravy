namespace Spravy.Ui.Services;

public class TaskProgressService : ITaskProgressService
{
    private readonly List<TaskProgressItem> items = new();
    
    [Inject]
    public required MainProgressBarViewModel MainProgressBar { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result<TaskProgressItem>> AddItemAsync(double impact)
    {
        var result = new TaskProgressItem(this, impact);
        items.Add(result);
        
        return this.InvokeUIBackgroundAsync(() => MainProgressBar.Maximum += impact)
           .IfSuccessAsync(() => result
               .AddDisposable(result.WhenAnyValue(x => x.Progress)
                   .Subscribe(x => MainProgressBar.Value = items.Sum(y => y.Progress)))
               .IfSuccess(() => result.AddDisposable(result.WhenAnyValue(x => x.Impact)
                   .Subscribe(x =>
                    {
                        if (Math.Abs(x - result.Progress) < 0.1)
                        {
                            DeleteItemAsync(result);
                        }
                    })))
               .IfSuccess(() => result.ToResult()), CancellationToken.None);
    }
    
    public ConfiguredValueTaskAwaitable<Result> DeleteItemAsync(TaskProgressItem item)
    {
        if (items.Remove(item))
        {
            item.Dispose();
            
            return this.InvokeUIBackgroundAsync(() => MainProgressBar.Maximum -= item.Impact);
        }
        
        return Result.AwaitableFalse;
    }
}