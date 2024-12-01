namespace Spravy.Domain.Models;

public class TreeVerticalStringHumanizingOptions
{
    public TreeVerticalStringHumanizingOptions()
    {
        Indent = 1;
        Format = "{0}{1}";
    }

    public ushort FirstIndent { get; set; }
    public ushort Indent { get; set; }
    public string Format { get; set; }
}