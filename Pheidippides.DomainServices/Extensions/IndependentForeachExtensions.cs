using System.Collections.Concurrent;

namespace Pheidippides.DomainServices.Extensions;

public static class IndependentForeachExtensions
{
    public static async Task IndependentParallelForEachAsync<T>(
        this IEnumerable<T> enumerable,
        Func<T, CancellationToken, Task> action,
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
            throw new AggregateException(exceptions);
    }
}