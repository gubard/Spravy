using System.Runtime.CompilerServices;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Picture.Db.Contexts;
using Spravy.Picture.Db.Models;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Service.Services;

public class PictureService : IPictureService
{
    private readonly IFactory<PictureSpravyDbContext> dbContextFactory;
    private readonly IFactory<IPictureEntryService> pictureEntryServiceFactory;

    public PictureService(
        IFactory<PictureSpravyDbContext> dbContextFactory,
        IFactory<IPictureEntryService> pictureEntryServiceFactory
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.pictureEntryServiceFactory = pictureEntryServiceFactory;
    }

    public ConfiguredValueTaskAwaitable<Result<PictureResponse>> GetPictureAsync(
        GetPicture getPicture,
        CancellationToken ct
    )
    {
        return pictureEntryServiceFactory.Create()
           .IfSuccessAsync(
                pictureEntryService => dbContextFactory.Create()
                   .IfSuccessDisposeAsync(
                        context => getPicture.Pictures.IfSuccessForEachAsync(
                            picture => picture.EntryIds.IfSuccessForEachAsync(
                                entryId => entryId.Ids.IfSuccessForEachAsync(
                                    id => pictureEntryService.GetEntriesAsync(
                                            entryId.Entry,
                                            id,
                                            picture.Size,
                                            picture.Type,
                                            ct
                                        )
                                       .IfSuccessForEachAsync(
                                            entry => context.GetEntityAsync<PictureItemEntity>(entry.Id)
                                               .IfSuccessAsync(
                                                    entity => new PictureItem(
                                                        entryId.Entry,
                                                        id,
                                                        new(
                                                            entity.Id,
                                                            entity.Name,
                                                            entity.Description,
                                                            entry.Data
                                                        )
                                                    ).ToResult(),
                                                    ct
                                                ),
                                            ct
                                        ),
                                    ct
                                ),
                                ct
                            ),
                            ct
                        ),
                        ct
                    ),
                ct
            )
           .IfSuccessAsync(items => new PictureResponse(items.SelectMany().SelectMany().SelectMany()).ToResult(), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> EditPictureAsync(EditPicture editPicture, CancellationToken ct)
    {
        return pictureEntryServiceFactory.Create()
           .IfSuccessAsync(
                pictureEntryService => dbContextFactory.Create()
                   .IfSuccessDisposeAsync(
                        context => context.AtomicExecuteAsync(
                            () =>
                                editPicture.AddPictures.IfSuccessForEachAsync(
                                    addPicture => addPicture.Items.IfSuccessForEachAsync(
                                        item => context.AddEntityAsync(
                                                new PictureItemEntity
                                                {
                                                    Id = Guid.NewGuid(),
                                                    Description = item.Description,
                                                    Name = item.Name,
                                                },
                                                ct
                                            )
                                           .IfSuccessAsync(
                                                entity =>
                                                    addPicture.EntryId.Ids.IfSuccessForEachAsync(
                                                        id => pictureEntryService.SaveAsync(
                                                            addPicture.EntryId.Entry,
                                                            id,
                                                            $"{entity.Entity.Id}.webp",
                                                            item.Data,
                                                            ct
                                                        ),
                                                        ct
                                                    ),
                                                ct
                                            ),
                                        ct
                                    ),
                                    ct
                                ),
                            ct
                        ),
                        ct
                    ),
                ct
            );
    }
}