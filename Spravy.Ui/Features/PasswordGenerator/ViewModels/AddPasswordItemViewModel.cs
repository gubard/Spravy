namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class AddPasswordItemViewModel : DialogableViewModelBase
{
    public override string ViewId => TypeCache<AddPasswordItemViewModel>.Type.Name;

    public AddPasswordItemViewModel(EditPasswordItemViewModel editPasswordItemViewModel)
    {
        EditPasswordItemViewModel = editPasswordItemViewModel;
    }

    public EditPasswordItemViewModel EditPasswordItemViewModel { get; }

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
}