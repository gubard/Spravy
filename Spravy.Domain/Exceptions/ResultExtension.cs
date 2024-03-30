using System.Text;
using Spravy.Domain.Models;

namespace Spravy.Domain.Exceptions;

public static class ResultExtension
{
    public static string GetTitle(this Result result)
    {
        var stringBuilder = new StringBuilder();

        foreach (var validationResult in result.ValidationResults.Span)
        {
            stringBuilder.Append(validationResult.Name);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }
}