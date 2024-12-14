namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordGeneratorViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly IPasswordItemCache passwordItemCache;
    private readonly IPasswordService passwordService;
    private readonly AppOptions appOptions;

    public PasswordGeneratorViewModel(
        IPasswordService passwordService,
        IPasswordItemCache passwordItemCache,
        AppOptions appOptions
    ) : base(true)
    {
        this.passwordService = passwordService;
        this.passwordItemCache = passwordItemCache;
        this.appOptions = appOptions;
    }

    public override string ViewId => TypeCache<PasswordGeneratorViewModel>.Name;

    public AvaloniaList<PasswordItemEntityNotify> Items { get; } = new();

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return passwordService.GetChildrenPasswordItemIdsAsync(OptionStruct<Guid>.Default, ct)
           .IfSuccessAsync(
                ids => ids.IfSuccessForEach(id => passwordItemCache.GetPasswordItem(id))
                   .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => Items.UpdateUi(items)), ct)
                   .IfSuccessAsync(
                        () => passwordService.GetPasswordItemsAsync(ids, appOptions.PasswordItemsChunkSize, ct)
                           .IfSuccessForEachAsync(
                                passwordItems => this.InvokeUiBackgroundAsync(
                                    () => passwordItems.IfSuccessForEach(i => passwordItemCache.UpdateUi(i))
                                ),
                                ct
                            ),
                        ct
                    ),
                ct
            );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}