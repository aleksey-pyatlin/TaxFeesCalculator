using System;
using System.Collections.Generic;
using FeesCalculator.ConsoleApplication.Adapters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class AdapterConfiguration<TConfigurator>
    {
        public TConfigurator Configurator { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public AdapterFactory Factory { get; set; }

        public List<String> Files { get; set; }

        [JsonIgnore]
        public Func<IAdapterConfiguration, IAdapter> Adapter { get; set; }
    }
}