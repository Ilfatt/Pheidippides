using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Pheidippides.DomainServices.Extensions;

public static class IndependentForeachExtensions
{
    public static async Task IndependentParallelForEachAsync<T>(
        this IEnumerable<T> enumerable,
        Func<T, CancellationToken, Task> action,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var exceptions = new ConcurrentBag<Exception>();

        await Parallel.ForEachAsync(
            enumerable,
            new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            },
            async (arg, token) =>
            {
                try
                {
                    await action(arg, token);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            });

        if (!exceptions.IsEmpty)
            logger.LogError(new AggregateException(exceptions), "Parallel action failed");
    }
}