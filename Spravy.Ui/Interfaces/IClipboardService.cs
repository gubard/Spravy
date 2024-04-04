using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IClipboardService
{
    ValueTask<Result> SetTextAsync(string? text);
}