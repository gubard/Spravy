namespace Spravy.Ui.ViewModels;

public class GroupToDoItemSettingsViewModel : NavigatableViewModelBase, IApplySettings
{
    public GroupToDoItemSettingsViewModel() : base(true)
    {
    }

    public override string ViewId
    {
        get => TypeCache<GroupToDoItemSettingsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}