using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IClipboardService
{
    Task SetTextAsync(string? text);
}