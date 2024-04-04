using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
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

    public async ValueTask<Result> SetTextAsync(string? text)
    {
        await clipboard.SetTextAsync(text);

        return Result.Success;
    }
}