using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text)
    {
        return Result.AwaitableFalse;
    }
}