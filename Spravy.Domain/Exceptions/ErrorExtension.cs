using System.Text;
using Spravy.Domain.Models;

namespace Spravy.Domain.Exceptions;

public static class ErrorExtension
{
    public static string GetTitle(this Error error)
    {
        var stringBuilder = new StringBuilder();

        foreach (var result in error.ValidationResults.Span)
        {
            stringBuilder.Append(result.Name);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }
}