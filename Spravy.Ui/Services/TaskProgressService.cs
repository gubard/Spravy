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
           .IfSuccessAsync(() => result.AddDisposable(result.WhenAnyValue(x => x.Progress)
                   .Subscribe(_ =>
                    {
                        var span = CollectionsMarshal.AsSpan(items);
                        
                        if (IsAllFinished(span))
                        {
                            items.Clear();
                            MainProgressBar.Value = 0;
                            MainProgressBar.Maximum = 0;
                        }
                        else
                        {
                            MainProgressBar.Value = GetAllProgress(span);
                        }
                    }))
               .IfSuccess(() => result.ToResult()), CancellationToken.None);
    }
    
    private bool IsAllFinished(Span<TaskProgressItem> span)
    {
        for (var index = 0; index < span.Length; index++)
        {
            if (!span[index].IsFinished)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private ushort GetAllProgress(Span<TaskProgressItem> span)
    {
        ushort result = 0;
        
        for (var index = 0; index < span.Length; index++)
        {
            result += span[index].Progress;
        }
        
        return result;
    }
}