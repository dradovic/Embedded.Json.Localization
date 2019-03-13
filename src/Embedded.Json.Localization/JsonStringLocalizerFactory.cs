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
            var cultureInfo = CultureInfo.CurrentUICulture;
            var resourceName = $"{resourceType.FullName}.json";
            return GetCachedLocalizer(resourceName, resourceType.Assembly, cultureInfo);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var cultureInfo = CultureInfo.CurrentUICulture;
            var resourceName = $"{baseName}.json";
            return GetCachedLocalizer(resourceName, Assembly.GetEntryAssembly(), cultureInfo);
        }

        private IStringLocalizer GetCachedLocalizer(string resourceName, Assembly assembly, CultureInfo cultureInfo)
        {
            string cacheKey = GetCacheKey(resourceName, assembly, cultureInfo);
            return _cache.GetOrAdd(cacheKey, new JsonStringLocalizer(resourceName, assembly, cultureInfo, _loggerFactory.CreateLogger<JsonStringLocalizer>()));
        }

        private string GetCacheKey(string resourceName, Assembly assembly, CultureInfo cultureInfo)
        {
            return resourceName + ';' + assembly.FullName + ';' + cultureInfo.Name;
        }
    }
}