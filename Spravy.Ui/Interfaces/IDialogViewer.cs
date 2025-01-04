namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    Cvtar ShowDialogAsync(DialogViewLayer layer, IDialogable dialogable, CancellationToken ct);
    Cvtar CloseDialogAsync(DialogViewLayer layer, CancellationToken ct);
    Result<bool> CloseLastDialog(CancellationToken ct);
}