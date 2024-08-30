namespace Spravy.Ui.Models;

public class ViewPipeline
{
    private readonly ReadOnlyMemory<INavigatable> views;
    private byte currentIndex;
    private byte maxIndex;

    public ViewPipeline(ReadOnlyMemory<INavigatable> views)
    {
        this.views = views;
    }

    public ConfiguredValueTaskAwaitable<Result<INavigatable>> NextViewAsync(CancellationToken ct)
    {
        return Result
            .AwaitableSuccess.IfSuccessAsync(
                () =>
                    currentIndex == 0
                        ? Result.AwaitableSuccess
                        : views.Span[currentIndex - 1].SaveStateAsync(ct),
                ct
            )
            .IfSuccessAsync(
                () =>
                {
                    if (currentIndex == maxIndex)
                    {
                        maxIndex++;

                        return views.Span[currentIndex].LoadStateAsync(ct);
                    }

                    return Result.AwaitableSuccess;
                },
                ct
            )
            .IfSuccessAsync(() => views.Span[currentIndex++].ToResult(), ct);
    }

    public Result<Option<INavigatable>> PreviousView()
    {
        return currentIndex == 0
            ? Option<INavigatable>.None.ToResult()
            : views.Span[--currentIndex].ToOption().ToResult();
    }
}
