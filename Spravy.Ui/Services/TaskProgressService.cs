namespace Spravy.Ui.Services;

public class TaskProgressService : ITaskProgressService
{
    private readonly List<TaskProgressItem> items = new();
    
    public TaskProgressService(MainProgressBarViewModel mainProgressBar)
    {
        MainProgressBar = mainProgressBar;
    }
    
    public  MainProgressBarViewModel MainProgressBar { get;  }
    
    public ConfiguredValueTaskAwaitable<Result<TaskProgressItem>> AddItemAsync(ushort impact)
    {
        var result = new TaskProgressItem(impact);
        items.Add(result);
        
        return this.InvokeUiBackgroundAsync(() =>
            {
                 MainProgressBar.Maximum += impact;
                 
                 return Result.Success;
            })
           .IfSuccessAsync(() =>
            {
                result.WhenAnyValue(x => x.Progress)
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
                    });
                
                return result.ToResult();
            }, CancellationToken.None);
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