namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordGeneratorViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly IPasswordItemCache passwordItemCache;
    private readonly IPasswordService passwordService;
    
    public PasswordGeneratorViewModel(
        IPasswordService passwordService,
        IPasswordItemCache passwordItemCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        this.passwordService = passwordService;
        this.passwordItemCache = passwordItemCache;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }
    
    public override string ViewId
    {
        get => TypeCache<PasswordGeneratorViewModel>.Type.Name;
    }
    
    public AvaloniaList<PasswordItemNotify> Items { get; } = new();
    public SpravyCommand InitializedCommand { get; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        Items.Clear();
        
        return passwordService.GetPasswordItemsAsync(ct)
           .IfSuccessForEachAsync(item => this.InvokeUiBackgroundAsync(() =>
            {
                Items.Add(passwordItemCache.GetPasswordItem(item.Id));
                passwordItemCache.UpdateAsync(item);
                
                return Result.Success;
            }), ct);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}