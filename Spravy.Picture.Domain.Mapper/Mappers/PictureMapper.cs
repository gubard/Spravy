using Google.Protobuf.Collections;
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
    public static partial EditPictureRequest ToEditPictureRequest(this EditPicture value);
    public static partial AddPictureItemGrpc ToAddPictureItemGrpc(this AddPictureItem value);

    public static GetPictureRequest ToGetPictureRequest(this GetPicture value)
    {
        var request = new GetPictureRequest();
        request.EntryIds.AddRange(value.EntryIds.Select(x => x.ToEntryIdGrpc()).ToArray());
        request.Parameters.AddRange(value.Parameters.Select(x => x.ToPictureParameterGrpc()).ToArray());

        return request;
    }

    public static PictureData ToPictureData(this PictureDataGrpc value)
    {
        var result = new PictureData(value.Id.ToGuid(), value.Data.ToByteArray());

        return result;
    }

    public static PictureParameter ToPictureParameter(this PictureParameterGrpc value)
    {
        var result = new PictureParameter(value.Id.ToGuid(), value.Entry, (SizeType)value.Type, (ushort)value.Size);

        return result;
    }

    public static PictureInfo ToPictureInfo(this PictureInfoGrpc value)
    {
        var result = new PictureInfo(value.Id.ToGuid(), value.Name, value.Description);

        return result;
    }

    public static PictureParameterGrpc ToPictureParameterGrpc(this PictureParameter value)
    {
        var result = new PictureParameterGrpc
        {
            Size = value.Size,
            Id = value.Id.ToByteString(),
            Type = (SizeTypeGrpc)value.Type,
            Entry = value.Entry,
        };

        return result;
    }

    public static PictureInfoGrpc ToPictureInfoGrpc(this PictureInfo value)
    {
        var result = new PictureInfoGrpc
        {
            Name = value.Name,
            Description = value.Description,
            Id = value.Id.ToByteString(),
        };

        return result;
    }

    public static PictureDataGrpc ToPictureDataGrpc(this PictureData value)
    {
        var result = new PictureDataGrpc
        {
            Data = value.Data.ToByteString(),
            Id = value.Id.ToByteString(),
        };

        return result;
    }

    public static PictureItemGrpc ToPictureItemGrpc(this PictureItem value)
    {
        var result = new PictureItemGrpc
        {
            Entry = value.Entry,
            Info = value.Info.ToPictureInfoGrpc(),
            Id = value.Id.ToByteString(),
        };

        return result;
    }

    public static PictureItem ToPictureItem(this PictureItemGrpc value)
    {
        var result = new PictureItem(value.Entry, value.Id.ToGuid(), value.Info.ToPictureInfo());

        return result;
    }

    public static PictureResponse ToPictureResponse(this GetPictureReply value)
    {
        var result = new PictureResponse(
            value.Pictures.Select(x => x.ToPictureItem()).ToArray(),
            value.Data.Select(x => x.ToPictureData()).ToArray()
        );

        return result;
    }

    public static GetPicture ToGetPicture(this GetPictureRequest value)
    {
        var result = new GetPicture(
            value.EntryIds.Select(x => x.ToEntryId()).ToArray(),
            value.Parameters.Select(x => x.ToPictureParameter()).ToArray()
        );

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