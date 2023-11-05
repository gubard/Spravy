using Spravy.Authentication.Service.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class BytesToStringFactory : IFactory<string, Named<IBytesToString>>
{
    public Named<IBytesToString> Create(string key)
    {
        if (NamedHelper.BytesToUpperCaseHexString.Name == key)
        {
            return NamedHelper.BytesToUpperCaseHexString;
        }

        throw new Exception($"Not found IBytesToString {key}.");
    }
}