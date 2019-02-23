using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Embedded.Json.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, string> _resources;
        private readonly ILogger<JsonStringLocalizer> _logger;

        public JsonStringLocalizer(string resourceName, Assembly resourceAssembly, ILogger<JsonStringLocalizer> logger)
        {
            _resources = ReadResources(resourceName, resourceAssembly, logger);
            _logger = logger;
        }

        private static Dictionary<string, string> ReadResources(string resourceName, Assembly resourceAssembly, ILogger<JsonStringLocalizer> logger)
        {
            Assembly satelliteAssembly;
            try
            {
                satelliteAssembly = resourceAssembly.GetSatelliteAssembly(CultureInfo.CurrentUICulture); // FIXME: da, check cases where the resource would be in the main assemlby
            }
            catch (FileNotFoundException x)
            {
                logger.LogWarning(x, x.Message);
                return new Dictionary<string, string>();
            }
            var stream = satelliteAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                logger.LogInformation($"Resource '{resourceName}' not found. Embedded in assembly?");
                return new Dictionary<string, string>();
            }
            string json;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (_resources.TryGetValue(name, out string value))
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
                if (_resources.TryGetValue(name, out string value))
                {
                    return new LocalizedString(name, string.Format(value, arguments), resourceNotFound: false);
                }
                return new LocalizedString(name, string.Format(name, arguments), resourceNotFound: true);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _resources.Select(r => new LocalizedString(r.Key, r.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new System.NotSupportedException();
        }
    }
}