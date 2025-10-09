namespace Spravy.Picture.Domain.Models;

public readonly struct PictureInfo
{
    public PictureInfo(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public readonly Guid Id;
    public readonly string Name;
    public readonly string Description;
}