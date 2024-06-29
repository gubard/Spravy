namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public class DeletePasswordItemViewModel : ViewModelBase
{
    public DeletePasswordItemViewModel()
    {
        this.WhenAnyValue(x => x.PasswordItemName)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DeleteText)));
    }

    [Reactive]
    public Guid PasswordItemId { get; set; }

    [Reactive]
    public string PasswordItemName { get; set; } = string.Empty;

    public Header4Localization DeleteText
    {
        get => new("DeletePasswordItemView.Header", new { PasswordItemName, });
    }
}
