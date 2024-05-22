namespace Spravy.Client.Services;

public class MetadataFactory : IMetadataFactory
{
    private readonly IHttpHeaderFactory httpHeaderFactory;

    public MetadataFactory(IHttpHeaderFactory httpHeaderFactory)
    {
        this.httpHeaderFactory = httpHeaderFactory;
    }

    public ConfiguredValueTaskAwaitable<Result<Metadata>> CreateAsync(CancellationToken cancellationToken)
    {
        return httpHeaderFactory.CreateHeaderItemsAsync(cancellationToken)
           .IfSuccessAsync(value =>
            {
                var metadata = new Metadata();

                foreach (var item in value.Span)
                {
                    metadata.Add(item.Name, item.Value);
                }

                return metadata.ToResult().ToValueTaskResult().ConfigureAwait(false);
            }, cancellationToken);
    }
}