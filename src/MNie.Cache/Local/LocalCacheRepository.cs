// ReSharper disable IdentifierTypo

using Microsoft.Extensions.Primitives;

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

    public class LocalCacheRepository<TItem, TKey>
        : ILocalCache<TItem, TKey>, IDisposable
        where TItem : class
    {
        internal IMemoryCache Cache;
        private CancellationTokenSource _tokenSource;
        private readonly TimeSpan _cacheEntryExpiration;

        public LocalCacheRepository(TimeSpan cacheEntryExpiration, long sizeLimit)
        {
            _tokenSource = new CancellationTokenSource();
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

        public Task<IResult<Unit>> DeleteAllAsync(CancellationToken token = default)
        {
            try
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();

                _tokenSource = new CancellationTokenSource();
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
                var opt = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = _cacheEntryExpiration,
                    Size = 1
                };
                opt.AddExpirationToken(new CancellationChangeToken(_tokenSource.Token));
                Cache.Set(key, item, opt);
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

        public void Dispose()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            Cache.Dispose();
        }
    }
}