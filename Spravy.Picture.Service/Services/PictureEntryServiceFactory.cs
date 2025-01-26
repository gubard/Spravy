using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Service.Models;
using Spravy.Service.Extensions;

namespace Spravy.Picture.Service.Services;

public class PictureEntryServiceFactory : IFactory<IPictureEntryService>
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly PictureOptions pictureOptions;
    private readonly ISpravyFileSystem spravyFileSystem;
    private readonly IPictureEditor pictureEditor;

    public PictureEntryServiceFactory(
        IHttpContextAccessor httpContextAccessor,
        PictureOptions pictureOptions,
        ISpravyFileSystem spravyFileSystem,
        IPictureEditor pictureEditor
    )
    {
        this.httpContextAccessor = httpContextAccessor;
        this.pictureOptions = pictureOptions;
        this.spravyFileSystem = spravyFileSystem;
        this.pictureEditor = pictureEditor;
    }

    public Result<IPictureEntryService> Create()
    {
        var userId = httpContextAccessor.GetUserId();

        var filesFolder = spravyFileSystem.GetFilesDirectory()
           .Combine(pictureOptions.Folder.ThrowIfNull())
           .Combine(userId);

        IPictureEntryService pictureEntryService = new PictureEntryService(filesFolder, pictureEditor);

        return pictureEntryService.ToResult();
    }
}