using System.Runtime.CompilerServices;
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

    public ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text)
    {
        return SetTextCore(text).ConfigureAwait(false);
    }

    public async ValueTask<Result> SetTextCore(string? text)
    {
        await clipboard.SetTextAsync(text);

        return Result.Success;
    }
}