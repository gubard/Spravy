using System.Threading.Tasks;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public Task SetTextAsync(string? text)
    {
        return Task.CompletedTask;
    }
}