namespace Spravy.Ui.Models;

public class TaskProgressItem : NotifyBase
{
    private readonly ITaskProgressService progressService;
    
    public TaskProgressItem(ITaskProgressService progressService, double impact)
    {
        Impact = impact;
        this.progressService = progressService;
    }
    
    public double Impact { get; }
    
    [Reactive]
    public double Progress { get; set; }
    
    public Result AddDisposable(IDisposable disposable)
    {
        Disposables.Add(disposable);
        
        return Result.Success;
    }
}