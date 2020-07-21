namespace MNie.Cache.Tests.Local
{
    using System;
    using System.Threading.Tasks;
    using Cache.Local;
    using Microsoft.Extensions.Caching.Memory;
    using ResultType;
    using ResultType.Results;
    using Shouldly;
    using Xunit;

    public class LocalCacheRepositoryTests
    {
        private static IMemoryCache CreateMemoryCache()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(20)
            });
            cache.Set("DataMockMock:2137", new DataMock {IntId = 2137, StringId = "ExistBefore"});
            return cache;
        }
        [Fact]
        public async Task When_deleting_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.DeleteAsync(2137);
            var existingEntries = await sut.GetAsync(2137); 
            
            (result is Success<Unit>).ShouldBeTrue();
            (existingEntries is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_deleting_not_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.DeleteAsync(1488);
            var existingEntries = await sut.GetAsync(2137); 
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) existingEntries).Payload.IntId.ShouldBe(2137);
        }
        
        [Fact]
        public async Task When_deleting_existing_entry_while_there_is_an_additional_entry_left()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.DeleteAsync(2137);
            var existingEntries = await sut.GetAsync(1488); 
            
            (result is Success<Unit>).ShouldBeTrue();
            (existingEntries is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_getting_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.GetAsync(2137); 
            
            (result is Success<DataMock>).ShouldBeTrue();
            ((Success<DataMock>) result).Payload.IntId.ShouldBe(2137);
        }
        
        [Fact]
        public async Task When_getting_expired_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromMilliseconds(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var _ = await sut.UpsertAsync(1488, new DataMock { IntId = 1488}); 
            await Task.Delay(6000);
            
            var result = await sut.GetAsync(1488); 
            
            (result is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_getting_not_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.GetAsync(1488); 
            
            (result is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_upserting_not_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.UpsertAsync(1488, new DataMock { IntId = 1}); 
            var fetch = await sut.GetAsync(1488);
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) fetch).Payload.IntId.ShouldBe(1);
        }
        
        [Fact]
        public async Task When_upserting_existing_entry()
        {
            var sut = new LocalCacheRepository<DataMock, int>(TimeSpan.FromDays(1), 200_000)
            {
                Cache = CreateMemoryCache()
            };
            var result = await sut.UpsertAsync(2137, new DataMock { IntId = 1488}); 
            var fetch = await sut.GetAsync(2137);
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) fetch).Payload.IntId.ShouldBe(1488);
        }
    }
}