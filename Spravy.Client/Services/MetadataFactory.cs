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

    public async Task<Metadata> CreateAsync(CancellationToken cancellationToken)
    {
        var metadata = new Metadata();
        cancellationToken.ThrowIfCancellationRequested();
        var items = await httpHeaderFactory.CreateHeaderItemsAsync(cancellationToken);

        foreach (var item in items)
        {
            metadata.Add(item.Name, item.Value);
        }
        
        return metadata;
    }
}