using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Service.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class HashServiceFactory : IFactory<string, Named<IHashService>>
{
    public Named<IHashService> Create(string key)
    {
        if (NamedHelper.Sha512Hash.Name == key)
        {
            return NamedHelper.Sha512Hash;
        }

        throw new Exception($"Not found IBytesToString {key}.");
    }
}