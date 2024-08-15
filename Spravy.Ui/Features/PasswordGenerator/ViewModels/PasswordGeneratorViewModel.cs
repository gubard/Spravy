namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordGeneratorViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly IPasswordItemCache passwordItemCache;
    private readonly IPasswordService passwordService;

    public PasswordGeneratorViewModel(
        IPasswordService passwordService,
        IPasswordItemCache passwordItemCache
    )
        : base(true)
    {
        this.passwordService = passwordService;
        this.passwordItemCache = passwordItemCache;
    }

    public override string ViewId
    {
        get => TypeCache<PasswordGeneratorViewModel>.Type.Name;
    }

    public AvaloniaList<PasswordItemEntityNotify> Items { get; } = new();

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        Items.Clear();

        return passwordService
            .GetPasswordItemsAsync(ct)
            .IfSuccessForEachAsync(
                item =>
                    passwordItemCache
                        .GetPasswordItem(item.Id)
                        .IfSuccessAsync(
                            i =>
                                this.InvokeUiBackgroundAsync(() =>
                                {
                                    Items.Add(i);
                                    passwordItemCache.UpdateUi(item);

                                    return Result.Success;
                                }),
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
