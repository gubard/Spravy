using Avalonia.Media.Imaging;

namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoImage : IDisposable
{
    Bitmap Data { get; }
}