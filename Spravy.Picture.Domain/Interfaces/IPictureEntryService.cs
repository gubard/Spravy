using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Enums;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Domain.Interfaces;

public interface IPictureEntryService
{
    Cvtar SaveAsync(string entry, Guid id, string name, Stream stream, CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<PictureEntry>> GetEntryAsync(
        string entry,
        Guid pictureId,
        double size,
        SizeType type,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PictureEntry>>> GetEntriesAsync(
        string entry,
        Guid id,
        double size,
        SizeType type,
        CancellationToken ct
    );
    
    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetEntriesAsync(
        string entry,
        Guid id,
        CancellationToken ct
    );
}