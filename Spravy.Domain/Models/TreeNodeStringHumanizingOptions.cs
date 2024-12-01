namespace Spravy.Domain.Models;

public class TreeNodeStringHumanizingOptions
{
    public TreeNodeStringHumanizingOptions()
    {
        Fromat = "{0} {1}";
    }

    public string Fromat { get; set; }
}