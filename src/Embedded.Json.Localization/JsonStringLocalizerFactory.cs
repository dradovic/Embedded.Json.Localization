using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Embedded.Json.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, IStringLocalizer> _cache = new ConcurrentDictionary<string, IStringLocalizer>();

        public JsonStringLocalizerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var resourceType = resourceSource.GetTypeInfo();
            var culture = CultureInfo.CurrentUICulture;
            var resourceName = $"{resourceType.FullName}.json";
            return GetCachedLocalizer(resourceName, resourceType.Assembly);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var culture = CultureInfo.CurrentUICulture;
            var resourceName = $"{baseName}.json";
            return GetCachedLocalizer(resourceName, Assembly.GetEntryAssembly());
        }

        private IStringLocalizer GetCachedLocalizer(string resourceName, Assembly assembly)
        {
            string cacheKey = GetCacheKey(resourceName, assembly);
            return _cache.GetOrAdd(cacheKey, new JsonStringLocalizer(resourceName, assembly, _loggerFactory.CreateLogger<JsonStringLocalizer>()));
        }

        private string GetCacheKey(string resourceName, Assembly assembly)
        {
            return resourceName + ';' + assembly.FullName;
        }
    }
}