namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class PasswordItemSettingsViewModel : DialogableViewModelBase, IApplySettings
{
    private readonly IPasswordService passwordService;

    public PasswordItemSettingsViewModel(
        PasswordItemEntityNotify item,
        EditPasswordItemViewModel editPasswordItemViewModel,
        IPasswordService passwordService
    )
    {
        Item = item;
        EditPasswordItemViewModel = editPasswordItemViewModel;
        this.passwordService = passwordService;
        EditPasswordItemViewModel.SetItemUi(item);
        EditPasswordItemViewModel.UndoAllUi();
    }

    public PasswordItemEntityNotify Item { get; }
    public EditPasswordItemViewModel EditPasswordItemViewModel { get; }
    public override string ViewId => TypeCache<PasswordItemSettingsViewModel>.Name;

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return passwordService.EditPasswordItemsAsync(
            EditPasswordItemViewModel.ToEditPasswordItems().SetIds(Item.Id.ToReadOnlyMemory()),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}