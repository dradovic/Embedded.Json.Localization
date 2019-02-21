using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Resources.Localization.Json
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public JsonStringLocalizerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            // FIXME: da, implement caching
            var resourceType = resourceSource.GetTypeInfo();
            var culture = CultureInfo.CurrentUICulture;
            var resourceName = $"{resourceType.FullName}.json";
            return new JsonStringLocalizer(resourceName, resourceType.Assembly, _loggerFactory.CreateLogger<JsonStringLocalizer>());
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var culture = CultureInfo.CurrentUICulture;
            var resourceName = $"{baseName}.json";
            return new JsonStringLocalizer(resourceName, Assembly.GetEntryAssembly(), _loggerFactory.CreateLogger<JsonStringLocalizer>());
        }
    }
}