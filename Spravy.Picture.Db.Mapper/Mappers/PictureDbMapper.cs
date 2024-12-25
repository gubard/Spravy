using System.Runtime.CompilerServices;
using Riok.Mapperly.Abstractions;
using Spravy.Picture.Db.Models;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public partial class PictureDbMapper
{
    public static PictureItemEntity ToPictureItemEntity(AddPicture add)
    {
        return new()
        {
            Name = add.Name,
            Description = add.Description,
        };
    }

    public static ConfiguredValueTaskAwaitable<Domain.Models.Picture> ToPictureAsync(
        PictureItemEntity entity,
        Stream stream
    )
    {
        return Domain.Models.Picture.CreateAsync(entity.Id, entity.Name, entity.Description, stream);
    }
}