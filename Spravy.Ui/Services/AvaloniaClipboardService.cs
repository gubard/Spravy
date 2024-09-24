namespace Spravy.Ui.Services;

public class AvaloniaClipboardService : IClipboardService
{
    private readonly Application application;

    public AvaloniaClipboardService(Application application)
    {
        this.application = application;
    }

    public Cvtar SetTextAsync(string? text, CancellationToken ct)
    {
        return application
            .GetTopLevel()
            .IfNotNull(nameof(TopLevel))
            .IfSuccess(toplevel => toplevel.Clipboard.IfNotNull(nameof(toplevel.Clipboard)))
            .IfSuccessAsync(clipboard => clipboard.SetTextAsync(text).Wrap(), ct);
    }
}
