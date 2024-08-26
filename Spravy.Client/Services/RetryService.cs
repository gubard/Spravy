namespace Spravy.Client.Services;

public class RetryService : IRetryService
{
    private readonly int retryCount = 3;

    public ConfiguredValueTaskAwaitable<Result<TReturn>> TryAsync<TReturn>(
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
        where TReturn : notnull
    {
        return TryCore(func).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> TryAsync(
        Func<ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        return TryCore(func).ConfigureAwait(false);
    }

    private async ValueTask<Result<TReturn>> TryCore<TReturn>(
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
        where TReturn : notnull
    {
        for (var i = 1; i <= retryCount; i++)
        {
            try
            {
                var result = await func.Invoke();

                if (!result.IsHasError)
                {
                    return result;
                }

                if (result.IsHasError)
                {
                    if (result.Errors.All(x => x.Id == CanceledByUserError.MainId))
                    {
                        return result;
                    }
                }

                if (retryCount == i)
                {
                    return result;
                }
            }
            catch (TaskCanceledException)
            {
                return Result<TReturn>.CanceledByUserError;
            }
            catch (RpcException rpc) when (rpc.StatusCode == StatusCode.Cancelled)
            {
                return Result<TReturn>.CanceledByUserError;
            }
            catch (GrpcException rpc)
                when (rpc is { InnerException: RpcException { StatusCode: StatusCode.Cancelled } })
            {
                return Result<TReturn>.CanceledByUserError;
            }
            catch
            {
                if (retryCount == i)
                {
                    throw;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Exp(i)));
        }

        return new(new UnknownError(Guid.Empty));
    }

    private async ValueTask<Result> TryCore(Func<ConfiguredValueTaskAwaitable<Result>> func)
    {
        for (var i = 1; i <= retryCount; i++)
        {
            try
            {
                var result = await func.Invoke();

                if (!result.IsHasError)
                {
                    return result;
                }

                if (result.IsHasError)
                {
                    if (result.Errors.All(x => x.Id == CanceledByUserError.MainId))
                    {
                        return result;
                    }
                }

                if (retryCount == i)
                {
                    return result;
                }
            }
            catch (TaskCanceledException)
            {
                return Result.CanceledByUserError;
            }
            catch (RpcException rpc) when (rpc.StatusCode == StatusCode.Cancelled)
            {
                return Result.CanceledByUserError;
            }
            catch (GrpcException rpc)
                when (rpc is { InnerException: RpcException { StatusCode: StatusCode.Cancelled } })
            {
                return Result.CanceledByUserError;
            }
            catch
            {
                if (retryCount == i)
                {
                    throw;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Exp(i)));
        }

        return new(new UnknownError(Guid.Empty));
    }
}
