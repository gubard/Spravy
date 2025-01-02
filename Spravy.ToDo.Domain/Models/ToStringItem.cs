namespace Spravy.ToDo.Domain.Models;

public readonly struct ToStringItem
{
    public ToStringItem(Guid id, string text)
    {
        Id = id;
        Text = text;
    }

    public readonly Guid Id;
    public readonly string Text;
}