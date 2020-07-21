namespace MNie.Cache.Tests.Single
{
    using System.Threading.Tasks;
    using Remote;
    using ResultType;
    using ResultType.Results;
    using Shouldly;
    using Xunit;

    public class SingleCacheRepositoryTests
    {
        [Fact]
        public async Task When_deleting_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.DeleteAsync(2137);
            var existingEntries = await sut.GetAsync(2137); 
            
            (result is Success<Unit>).ShouldBeTrue();
            (existingEntries is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_deleting_not_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.DeleteAsync(1488);
            var existingEntries = await sut.GetAsync(2137); 
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) existingEntries).Payload.IntId.ShouldBe(2137);
        }
        
        [Fact]
        public async Task When_deleting_existing_entry_while_there_is_an_additional_entry_left()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.DeleteAsync(2137);
            var existingEntries = await sut.GetAsync(1488); 
            
            (result is Success<Unit>).ShouldBeTrue();
            (existingEntries is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_getting_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.GetAsync(2137); 
            
            (result is Success<DataMock>).ShouldBeTrue();
            ((Success<DataMock>) result).Payload.IntId.ShouldBe(2137);
        }
        
        [Fact]
        public async Task When_getting_not_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.GetAsync(1488); 
            
            (result is Failure<DataMock>).ShouldBeTrue();
        }
        
        [Fact]
        public async Task When_upserting_not_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.UpsertAsync(1488, new DataMock { IntId = 1}); 
            var fetch = await sut.GetAsync(1488);
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) fetch).Payload.IntId.ShouldBe(1);
        }
        
        [Fact]
        public async Task When_upserting_existing_entry()
        {
            var mock = new DistributedCacheMock();
            var sut = new RemoteCacheRepository<DataMock, int>(mock);
            
            var result = await sut.UpsertAsync(2137, new DataMock { IntId = 1488}); 
            var fetch = await sut.GetAsync(2137);
            
            (result is Success<Unit>).ShouldBeTrue();
            ((Success<DataMock>) fetch).Payload.IntId.ShouldBe(1488);
        }
    }
}