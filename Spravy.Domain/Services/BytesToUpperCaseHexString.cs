using System.Text;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class BytesToUpperCaseHexString : IBytesToString
{
    public string BytesToString(byte[] input)
    {
        // Create a new StringBuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        foreach (var value in input)
        {
            sBuilder.Append(value.ToString("X2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}