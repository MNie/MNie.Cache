// ReSharper disable IdentifierTypo
// ReSharper disable MethodHasAsyncOverload
namespace MNie.Cache.Remote
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Errors;
    using Microsoft.Extensions.Caching.Distributed;
    using ResultType;
    using ResultType.Factories;
    using ResultType.Results;
    using static Serialization.ByteSerializer;

    public class RemoteCacheRepository<TItem, TKey> : IRemoteCache<TItem, TKey>
    {
        private readonly IDistributedCache _distributedCache;

        public RemoteCacheRepository(IDistributedCache distributedCache) => _distributedCache = distributedCache;

        private static string GetKey(TKey id) => $"{typeof(TItem).Name}:{id}";

        public async Task<IResult<TItem>> GetAsync(TKey id, CancellationToken token = default)
        {
            try
            {
                var cacheResult = await _distributedCache.GetAsync(GetKey(id), token);

                if (cacheResult.Length > 0)
                    return Deserialize<TItem>(cacheResult);

                return ResultFactory.CreateFailure<TItem>(
                    new NotFoundError($"Cache {nameof(TItem)} entry for id: {id} was not found."));
            }
            catch (InvalidOperationException)
            {
                return ResultFactory.CreateFailure<TItem>(
                    new NotFoundError($"Cache {nameof(TItem)} entry for id: {id} was not found."));
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailure<TItem>(
                    $"Error occured while trying to get {nameof(TItem)} entry with id: {id} from cache. {e.Message}");
            }
        }

        public async Task<IResult<Unit>> DeleteAsync(TKey id, CancellationToken token = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(GetKey(id), token);
                return ResultFactory.CreateSuccess();
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailure<Unit>(e.Message);
            }
        }

        public async Task<IResult<Unit>> UpsertAsync(TKey id, TItem item, CancellationToken token = default)
        {
            try
            {
                var serialized = Serialize(item);
                await _distributedCache.SetAsync(GetKey(id), serialized, token);
                return ResultFactory.CreateSuccess();
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailure(e.Message);
            }
        }

        public async Task<IResult<Unit>> UpsertAsync(TKey id, TItem item, DistributedCacheEntryOptions opt, CancellationToken token = default)
        {
            try
            {
                var serialized = Serialize(item);
                await _distributedCache.SetAsync(GetKey(id), serialized, opt, token);
                return ResultFactory.CreateSuccess();
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailure(e.Message);
            }
        }
    }
}