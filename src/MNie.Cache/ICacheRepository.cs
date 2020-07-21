// ReSharper disable IdentifierTypo

namespace MNie.Cache
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    using ResultType;
    using ResultType.Results;

    public interface IRemoteCache<TItem, in TKey> : ICacheRepository<TItem, TKey>
    {
        Task<IResult<Unit>> UpsertAsync(TKey id, TItem item, DistributedCacheEntryOptions opt, CancellationToken token = default);
    }
    public interface ILocalCache<TItem, in TKey> : ICacheRepository<TItem, TKey> { }
    
    public interface ICacheRepository<TInput, in TKey>
    {
        Task<IResult<TInput>> GetAsync(TKey id, CancellationToken token = default);
        Task<IResult<Unit>> UpsertAsync(TKey id, TInput item, CancellationToken token = default);
        Task<IResult<Unit>> DeleteAsync(TKey id, CancellationToken token = default);
    }
}