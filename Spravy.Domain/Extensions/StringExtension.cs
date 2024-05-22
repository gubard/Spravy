namespace Spravy.Domain.Extensions;

public static class StringExtension
{
    public static string? GetEnvironmentVariable(this string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    public static Guid ToGuid(this string str)
    {
        return Guid.Parse(str);
    }

    public static Uri ToUri(this string str)
    {
        return new(str);
    }

    public static Result<string> CheckNullOrWhiteSpaceProperty(this string? propertyValue, string propertyName)
    {
        if (propertyValue is null)
        {
            return new(new PropertyNullValueError(propertyName));
        }

        if (propertyValue == string.Empty)
        {
            return new(new PropertyEmptyStringError(propertyName));
        }

        for (var index = 0; index < propertyValue.Length; ++index)
        {
            if (!char.IsWhiteSpace(propertyValue[index]))
            {
                return new(propertyValue);
            }
        }

        return new(new PropertyWhiteSpaceStringError(propertyName));
    }

    public static string ThrowIfNullOrWhiteSpace(
        this string? str,
        [CallerArgumentExpression(nameof(str))] string paramName = ""
    )
    {
        str = str.ThrowIfNull(paramName);

        if (str.IsNullOrWhiteSpace())
        {
            throw new WhiteSpaceException(paramName);
        }

        return str;
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static FileInfo ToFile(this string path)
    {
        return new(path);
    }

    public static DirectoryInfo ToDirectory(this string path)
    {
        return new(path);
    }

    public static string ToConsoleLine(this string line)
    {
        Console.WriteLine(line);

        return line;
    }

    public static async Task<MemoryStream> ToStreamAsync(this string str, Encoding encoding)
    {
        var bytes = str.ToByteArray(encoding);
        var stream = new MemoryStream();
        await stream.WriteAsync(bytes, 0, bytes.Length);

        return stream;
    }

    public static Task<MemoryStream> ToStreamAsync(this string str)
    {
        return str.ToStreamAsync(Encoding.UTF8);
    }

    public static byte[] ToByteArray(this string str, Encoding encoding)
    {
        return encoding.GetBytes(str);
    }

    public static byte[] ToByteArray(this string str)
    {
        return str.ToByteArray(Encoding.UTF8);
    }

    public static string ToWriteToFile(this string str, FileInfo file, Encoding encoding)
    {
        if (file.Exists)
        {
            file.Delete();
        }

        using var stream = file.Create();
        var bytes = encoding.GetBytes(str);
        stream.Write(bytes, 0, bytes.Length);

        return str;
    }

    public static string ToWriteToFile(this string str, FileInfo file)
    {
        return str.ToWriteToFile(file, Encoding.UTF8);
    }

    public static string? AddLine(this string? str, string? line)
    {
        if (str is null)
        {
            return line;
        }

        if (line is null)
        {
            return str;
        }

        return $"{str}{line}{Environment.NewLine}";
    }

    public static string Duplicate(this string str, ulong count)
    {
        if (count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        for (var i = 0ul; i < count; i++)
        {
            builder.Append(str);
        }

        return builder.ToString();
    }

    public static void Duplicate(this StringBuilder builder, string str, ulong count)
    {
        if (count == 0)
        {
            return;
        }

        for (var i = 0ul; i < count; i++)
        {
            builder.Append(str);
        }
    }

    public static TEnum ParseEnum<TEnum>(this string str) where TEnum : struct
    {
        return Enum.Parse<TEnum>(str);
    }
}