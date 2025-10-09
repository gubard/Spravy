using System.Runtime.CompilerServices;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Enums;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Service.Services;

public class PictureEntryService : IPictureEntryService
{
    private readonly DirectoryInfo root;
    private readonly IPictureEditor pictureEditor;

    public PictureEntryService(DirectoryInfo root, IPictureEditor pictureEditor)
    {
        this.root = root;
        this.pictureEditor = pictureEditor;
    }

    public ConfiguredValueTaskAwaitable<Result> SaveAsync(
        string entry,
        Guid id,
        string name,
        Stream stream,
        CancellationToken ct
    )
    {
        return SaveCore(
                entry,
                id,
                name,
                stream,
                ct
            )
           .ConfigureAwait(false);
    }
    public ConfiguredValueTaskAwaitable<Result<PictureEntry>> GetEntryAsync(
        string entry,
        Guid pictureId,
        double size,
        SizeType type,
        CancellationToken ct
    )
    {
        throw new NotImplementedException();
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PictureEntry>>> GetEntriesAsync(
        string entry,
        Guid id,
        double size,
        SizeType type,
        CancellationToken ct
    )
    {
        return GetEntriesCore(
                entry,
                id,
                size,
                type,
                ct
            )
           .ConfigureAwait(false);
    }
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetEntriesAsync(
        string entry,
        Guid id,
        CancellationToken ct
    )
    {
        var folder = root.Combine(entry).Combine(id.ToString());

        if (!folder.Exists)
        {
            folder.Create();
        }

        var names = folder.GetFiles().Select(x => Guid.Parse(x.GetFileNameWithoutExtension())).ToArray();

        return names.ToReadOnlyMemory().ToResult().ToValueTaskResult().ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<PictureEntry>>> GetEntriesCore(
        string entry,
        Guid id,
        double size,
        SizeType type,
        CancellationToken ct
    )
    {
        var folder = root.Combine(entry).Combine(id.ToString());

        if (!folder.Exists)
        {
            folder.Create();
        }

        var names = folder.GetFiles().Select(x => x.GetFileNameWithoutExtension()).ToArray();
        var result = new PictureEntry[names.Length];

        for (var index = 0; index < names.Length; index++)
        {
            var name = names[index];
            var imageFolder = folder.Combine(name);
            var file = imageFolder.ToFile($"{type}.{size}.webp");

            if (file.Exists)
            {
                result[index] = new(Guid.Parse(name), await file.OpenRead().ToByteArrayAsync());

                continue;
            }

            if (!imageFolder.Exists)
            {
                imageFolder.Create();
            }

            await using var imageStream = folder.ToFile($"{name}.webp").OpenRead();
            using var stream = new MemoryStream();
            await imageStream.CopyToAsync(stream, ct);
            stream.Seek(0, SeekOrigin.Begin);

            (await pictureEditor.ResizeImageAsync(
                stream,
                size,
                type,
                file,
                ct
            )).ThrowIfError();

            result[index] = new(Guid.Parse(name), await file.OpenRead().ToByteArrayAsync());
        }

        return result.ToReadOnlyMemory().ToResult();
    }

    private async ValueTask<Result> SaveCore(
        string entry,
        Guid id,
        string name,
        Stream stream,
        CancellationToken ct
    )
    {
        var folder = root.Combine(entry).Combine(id.ToString());

        if (!folder.Exists)
        {
            folder.Create();
        }

        var file = folder.ToFile(name);
        await using var fileStream = file.Create();
        await stream.CopyToAsync(fileStream, ct);

        return Result.Success;
    }
}