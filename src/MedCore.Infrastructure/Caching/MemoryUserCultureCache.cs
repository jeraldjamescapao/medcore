namespace MedCore.Infrastructure.Caching;

using MedCore.Common.Caching;
using MedCore.Common.Http;
using Microsoft.Extensions.Caching.Memory;

internal sealed class MemoryUserCultureCache : IUserCultureCache
{
    private readonly IMemoryCache _cache;

    public MemoryUserCultureCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void InvalidateForUser(Guid userId)
    {
        _cache.Remove(CacheKeys.UserCulture(userId));
    }
}