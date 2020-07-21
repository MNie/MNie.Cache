// ReSharper disable IdentifierTypo
namespace MNie.Cache.Local
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Errors;
    using Microsoft.Extensions.Caching.Memory;
    using ResultType;
    using ResultType.Extensions;
    using ResultType.Factories;
    using ResultType.Results;

    public class LocalCacheRepository<TItem, TKey> : ILocalCache<TItem, TKey>
        where TItem : class
    {
        internal IMemoryCache Cache;
        private readonly TimeSpan _cacheEntryExpiration;

        public LocalCacheRepository(TimeSpan cacheEntryExpiration, long sizeLimit)
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = sizeLimit,
                ExpirationScanFrequency = TimeSpan.FromSeconds(5)
            });
            _cacheEntryExpiration = cacheEntryExpiration;
        }

        private static string GetKey(TKey id) => $"{typeof(TItem).Name}:{id}";
        
        public Task<IResult<TItem>> GetAsync(TKey id, CancellationToken token = default)
        {
            var item = Cache.Get<TItem>(GetKey(id));

            return item != default
                ? item.ToSuccessAsync()
                : ResultFactory.CreateFailureAsync<TItem>(
                    new NotFoundError($"Cache entry for {nameof(TItem)} with {id} was not found."));
        }

        public Task<IResult<Unit>> DeleteAsync(TKey id, CancellationToken token = default)
        {
            try
            {
                Cache.Remove(GetKey(id));
                return ResultFactory.CreateSuccessAsync();
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailureAsync<Unit>(e.Message);
            }
        }

        public Task<IResult<Unit>> UpsertAsync(TKey id, TItem item, CancellationToken token = default)
        {
            try
            {
                var key = GetKey(id);
                Cache.Set(key, item, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = _cacheEntryExpiration,
                    Size = 1
                });
                var successSave = Cache.TryGetValue(key, out _);
                return successSave
                    ? ResultFactory.CreateSuccessAsync()
                    : ResultFactory.CreateFailureAsync<Unit>($"entry under key {key} was not saved in a cache");
            }
            catch (Exception e)
            {
                return ResultFactory.CreateFailureAsync(e.Message);
            }
        }
    }
}