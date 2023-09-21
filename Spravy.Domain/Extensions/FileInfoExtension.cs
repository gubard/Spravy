namespace Spravy.Domain.Extensions;

public static class FileInfoExtension
{
    public static Task<string> ReadAllTextAsync(this FileInfo file)
    {
        return File.ReadAllTextAsync(file.FullName);
    }
    
    public static Task WriteAllTextAsync(this FileInfo file, string text)
    {
        return File.WriteAllTextAsync(file.FullName, text);
    }
    
    public static string GetFileNameWithoutExtension(this FileInfo file)
    {
        return Path.GetFileNameWithoutExtension(file.FullName);
    }
}