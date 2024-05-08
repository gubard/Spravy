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
    
    public Result AddDisposable(IDisposable disposable)
    {
        Disposables.Add(disposable);
        
        return Result.Success;
    }
    
    public void Finish()
    {
        Progress = Impact;
    }
}