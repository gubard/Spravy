using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class CodeClipboardService : IClipboardService
{
    public ValueTask<Result> SetTextAsync(string? text)
    {
        return Result.SuccessValueTask;
    }
}