using Newtonsoft.Json;

namespace FeesCalculator.ConsoleApplication.Utils
{
    internal class JsonUtils
    {
        public static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            jsonSerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());

            return jsonSerializerSettings;

        }
    }
}