using Grpc.Core;
using Spravy.Client.Interfaces;
using Spravy.Domain.Interfaces;

namespace Spravy.Client.Services;

public class MetadataFactory : IMetadataFactory
{
    private readonly IHttpHeaderFactory httpHeaderFactory;

    public MetadataFactory(IHttpHeaderFactory httpHeaderFactory)
    {
        this.httpHeaderFactory = httpHeaderFactory;
    }

    public async Task<Metadata> CreateAsync()
    {
        var metadata = new Metadata();
        var items = await httpHeaderFactory.CreateHeaderItemsAsync();

        foreach (var item in items)
        {
            metadata.Add(item.Name, item.Value);
        }
        
        return metadata;
    }
}