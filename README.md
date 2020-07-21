# MNie.Cache

* NuGet Status

    |   | MNie.Cache |
    |---|---|
    | nuget | [![NuGet](https://buildstats.info/nuget/MNie.Cache?includePreReleases=true)](https://www.nuget.org/packages/MNie.Cache) |


* Build Status
    [![Build Status](https://travis-ci.org/MNie/MNie.Cache.svg?branch=master)](https://travis-ci.org/MNie/MNie.Cache)

# MNie.Cache
Simple abstraction over IMemoryCache and IDistributedCache

Could be downloaded from NuGet:
```Install-Package MNie.Cache```

Simple usage - Memory Cache:

* Creation
```csharp
var localCache = new LocalCacheRepository<TData, TKey>(
  cacheEntryExpiration: TimeSpan.FromDays(1),
  sizeLimit: 200_000);
```

* Get value
```csharp
IResult<TData> value = await localCache.GetAsync(key, token);
```

* Delete value
```csharp
IResult<TData> value = await localCache.DeleteAsync(key, token);
```

* Add/Update value
```csharp
IResult<TData> value = await localCache.UpsertAsync(key, item, token);
```

Simple usage - distributed cache:

* Creation
```csharp
IDistributedCache cache = ...
var remoteCache = new RemoteCacheRepository<TData, TKey>(cache);
```

* Get value
```csharp
IResult<TData> value = await remoteCache.GetAsync(key, token);
```

* Delete value
```csharp
IResult<TData> value = await remoteCache.DeleteAsync(key, token);
```

* Add/Update value
```csharp
IResult<TData> value = await remoteCache.UpsertAsync(key, item, token);
```

