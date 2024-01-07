using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using Spravy.Domain.Extensions;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class TopLevelClipboardService : IClipboardService
{
    private readonly IClipboard clipboard = Application.Current.ThrowIfNull("Application")
        .GetTopLevel()
        .ThrowIfNull("TopLevel")
        .Clipboard
        .ThrowIfNull();

    public Task SetTextAsync(string? text)
    {
        return clipboard.SetTextAsync(text);
    }
}