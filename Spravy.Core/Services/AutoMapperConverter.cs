namespace Spravy.Core.Services;

public class AutoMapperConverter : IConverter
{
    private readonly IMapper mapper;

    public AutoMapperConverter(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public Result<TResult> Convert<TResult>(object? source)  where TResult : notnull
    {
        return mapper.Map<TResult>(source).ToResult();
    }
}