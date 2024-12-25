namespace Spravy.Domain.Models;

public readonly struct SizeDouble2D
{
    public SizeDouble2D(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Width { get; }
    public double Height { get; }
}