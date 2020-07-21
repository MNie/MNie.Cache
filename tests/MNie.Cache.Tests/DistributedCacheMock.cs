namespace MNie.Cache.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    using Serialization;

    [Serializable]
    public class DataMock
    {
        public int IntId { get; set; }
        public string StringId { get; set; }
    }
    
    public class DistributedCacheMock : IDistributedCache
    {
        private readonly IDictionary<string, byte[]> _dict = new Dictionary<string,byte[]>
        {
            [ "DataMock:2137" ] = ByteSerializer.Serialize(new DataMock { IntId = 2137, StringId = "ExistBefore" })
        };

        public byte[] Get(string key) => _dict.ContainsKey(key) ? _dict[key] : new byte[0];

        public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken()) =>
            Task.FromResult(_dict.ContainsKey(key) ? _dict[key] : new byte[0]);

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => _dict[key] = value;

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            _dict[key] = value;
            return Task.CompletedTask;
        }

        public void Refresh(string key) => throw new NotImplementedException();

        public Task RefreshAsync(string key, CancellationToken token = new CancellationToken()) =>
            throw new NotImplementedException();

        public void Remove(string key) => _dict.Remove(key);

        public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            _dict.Remove(key);
            return Task.CompletedTask;
        }
    }
}