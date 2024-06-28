namespace Spravy.Domain.Exceptions;

public class WhiteSpaceException : Exception
{
    public WhiteSpaceException(string name)
        : base($"{name} can't be white space.")
    {
        Name = name;
    }

    public string Name { get; }
}
