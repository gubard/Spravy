using Spravy.Domain.Interfaces;

namespace Spravy.Picture.Service.Models;

public class PictureOptions : IOptionsValue
{
    public static string Section => "Picture";

    public string? Folder { get; set; }
}