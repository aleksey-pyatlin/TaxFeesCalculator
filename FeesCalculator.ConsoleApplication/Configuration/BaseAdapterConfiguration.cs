using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    [Serializable]
    public class BaseAdapterConfiguration : IAdapterConfiguration
    {
        public String RootFolder { get; set; }

        [JsonExtensionData]
        public JObject ExtensionData { get; set; }
    }
}