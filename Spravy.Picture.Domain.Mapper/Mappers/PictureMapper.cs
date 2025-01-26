using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Domain.Extensions;
using Spravy.Picture.Domain.Enums;
using Spravy.Picture.Domain.Models;
using Spravy.Picture.Protos;

namespace Spravy.Picture.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class PictureMapper
{
    public static partial GetPictureReply ToGetPictureReply(this PictureResponse value);
    public static partial IdPictureParametersGrpc ToIdPictureParametersGrpc(this IdPictureParameters value);
    public static partial GetPictureRequest ToGetPictureRequest(this GetPicture value);
    public static partial EditPictureRequest ToEditPictureRequest(this EditPicture value);
    public static partial AddPictureItemGrpc ToAddPictureItemGrpc(this AddPictureItem value);

    public static PictureItemGrpc ToPictureItemGrpc(this PictureItem value)
    {
        var result = new PictureItemGrpc
        {
            Entry = value.Entry,
            Picture = value.Picture.ToPictureGrpc(),
            Id = value.Id.ToByteString(),
        };

        return result;
    }

    public static PictureGrpc ToPictureGrpc(this Models.Picture value)
    {
        var result = new PictureGrpc
        {
            Id = value.Id.ToByteString(),
            Name = value.Name,
            Description = value.Description,
            Data = value.Data.ToByteString(),
        };

        return result;
    }

    public static Models.Picture ToPicture(this PictureGrpc value)
    {
        var result = new Models.Picture(
            value.Id.ToGuid(),
            value.Name,
            value.Description,
            value.Data.ToByteArray().ToMemoryStream()
        );

        return result;
    }

    public static PictureItem ToPictureItem(this PictureItemGrpc value)
    {
        var result = new PictureItem(value.Entry, value.Id.ToGuid(), value.Picture.ToPicture());

        return result;
    }

    public static PictureResponse ToPictureResponse(this GetPictureReply value)
    {
        var result = new PictureResponse(value.Pictures.Select(x => x.ToPictureItem()).ToArray());

        return result;
    }

    public static IdPictureParameters ToIdPictureParameters(this IdPictureParametersGrpc value)
    {
        var result = new IdPictureParameters(
            value.EntryIds.Select(x => x.ToEntryId()).ToArray(),
            value.Size,
            (SizeType)value.Type
        );

        return result;
    }

    public static GetPicture ToGetPicture(this GetPictureRequest value)
    {
        var result = new GetPicture(value.Pictures.Select(x => x.ToIdPictureParameters()).ToArray());

        return result;
    }

    public static AddPictureGrpc ToAddPictureGrpc(this AddPicture value)
    {
        var result = new AddPictureGrpc
        {
            Description = value.Description,
            Name = value.Name,
            Data = value.Data.ToByteString(),
        };

        return result;
    }

    public static EntryIdGrpc ToEntryIdGrpc(this EntryId value)
    {
        var result = new EntryIdGrpc
        {
            Entry = value.Entry,
        };

        result.Ids.AddRange(value.Ids.Select(x => x.ToByteString()).ToArray());

        return result;
    }

    public static AddPicture ToAddPicture(this AddPictureGrpc value)
    {
        var result = new AddPicture(value.Name, value.Description, value.Data.ToByteArray().ToMemoryStream());

        return result;
    }

    public static EntryId ToEntryId(this EntryIdGrpc value)
    {
        var result = new EntryId(value.Entry, value.Ids.Select(x => x.ToGuid()).ToArray());

        return result;
    }

    public static AddPictureItem ToAddPictureItem(this AddPictureItemGrpc value)
    {
        var result = new AddPictureItem(value.EntryId.ToEntryId(), value.Items.Select(x => x.ToAddPicture()).ToArray());

        return result;
    }

    public static EditPicture ToEditPicture(this EditPictureRequest value)
    {
        var result = new EditPicture(value.AddPictures.Select(x => x.ToAddPictureItem()).ToArray());

        return result;
    }
}