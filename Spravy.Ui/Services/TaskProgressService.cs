namespace Spravy.Ui.Services;

public class TaskProgressService : ITaskProgressService
{
    private readonly List<TaskProgressItem> items = new();
    
    [Inject]
    public required MainProgressBarViewModel MainProgressBar { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result<TaskProgressItem>> AddItemAsync(ushort impact)
    {
        var result = new TaskProgressItem(impact);
        items.Add(result);
        
        return this.InvokeUIBackgroundAsync(() => MainProgressBar.Maximum += impact)
           .IfSuccessAsync(() => result
               .AddDisposable(result.WhenAnyValue(x => x.Progress)
                   .Subscribe(x => MainProgressBar.Value = items.Sum(y => y.Progress)))
               .IfSuccess(() => result.AddDisposable(result.WhenAnyValue(x => x.Impact)
                   .Subscribe(x =>
                    {
                        if (x == result.Progress)
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
            
            if (items.Count == 0)
            {
                return this.InvokeUIBackgroundAsync(() =>
                {
                    MainProgressBar.Maximum = 0;
                    MainProgressBar.Value = 0;
                });
            }
            
            return this.InvokeUIBackgroundAsync(() => MainProgressBar.Maximum -= item.Impact);
        }
        
        return Result.AwaitableFalse;
    }
}