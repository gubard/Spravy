using System.Text;
using Spravy.Domain.Models;

namespace Spravy.Domain.Exceptions;

public static class ResultExtension
{
    public static string GetTitle(this Result result)
    {
        var stringBuilder = new StringBuilder();

        foreach (var validationResult in result.Errors.Span)
        {
            stringBuilder.Append(validationResult.Message);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }
    
    public static string GetTitle<TValue>(this Result<TValue> result)
    {
        var stringBuilder = new StringBuilder();

        foreach (var validationResult in result.Errors.Span)
        {
            stringBuilder.Append(validationResult.Message);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }
}