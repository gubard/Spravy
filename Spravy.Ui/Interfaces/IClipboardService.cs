using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface IClipboardService
{
    ConfiguredValueTaskAwaitable<Result> SetTextAsync(string? text);
}