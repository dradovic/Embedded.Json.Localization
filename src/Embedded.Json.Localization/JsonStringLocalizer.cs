using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Embedded.Json.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly Lazy<IDictionary<string, string>> _resources;
        private readonly Lazy<IDictionary<string, string>> _fallbackResources;

        public JsonStringLocalizer(string resourceName, Assembly resourceAssembly, CultureInfo cultureInfo, ILogger<JsonStringLocalizer> logger)
        {
            _resources = new Lazy<IDictionary<string, string>>(
                () => ReadResources(resourceName, resourceAssembly, cultureInfo, logger, isFallback: false));
            _fallbackResources = new Lazy<IDictionary<string, string>>(
                () => ReadResources(resourceName, resourceAssembly, cultureInfo.Parent, logger, isFallback: true));
        }

        private static IDictionary<string, string> ReadResources(string resourceName, Assembly resourceAssembly, CultureInfo cultureInfo, ILogger<JsonStringLocalizer> logger, bool isFallback)
        {
            Assembly satelliteAssembly;
            try
            {
                satelliteAssembly = resourceAssembly.GetSatelliteAssembly(cultureInfo);
            }
            catch (FileNotFoundException x)
            {
                logger.LogInformation($"Could not find satellite assembly for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}': {x.Message}");
                return new Dictionary<string, string>();
            }
            var stream = satelliteAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                logger.LogInformation($"Resource '{resourceName}' not found for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}'.");
                return new Dictionary<string, string>();
            }
            string json;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            return JsonSerializer.Deserialize<IDictionary<string, string>>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
            }) ?? throw new InvalidOperationException($"Failed to parse JSON: '{json}'");
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (TryGetResource(name, out string value))
                {
                    return new LocalizedString(name, value, resourceNotFound: false);
                }
                return new LocalizedString(name, name, resourceNotFound: true);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (TryGetResource(name, out string value))
                {
                    return new LocalizedString(name, string.Format(value, arguments), resourceNotFound: false);
                }
                return new LocalizedString(name, string.Format(name, arguments), resourceNotFound: true);
            }
        }

        private bool TryGetResource(string name, out string value)
        {
            return _resources.Value.TryGetValue(name, out value) ||
                _fallbackResources.Value.TryGetValue(name, out value);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _resources.Value.Select(r => new LocalizedString(r.Key, r.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotSupportedException("Obsolete API. See: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.localization.istringlocalizer.withculture");
        }
    }
}