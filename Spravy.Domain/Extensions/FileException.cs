namespace Spravy.Domain.Extensions;

public class FileException : Exception
{
    public FileException(string message, Exception innerException, FileInfo file) : base(message, innerException)
    {
        File = file;
    }

    public FileInfo File { get; }
}