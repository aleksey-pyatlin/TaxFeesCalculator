using System;
using System.Collections.Generic;
using FeesCalculator.ConsoleApplication.Adapters;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class AdapterConfiguration<TConfigurator>
    {
        public TConfigurator Configurator { get; set; }

        public String AdapterFullType { get; set; }

        public List<String> Files { get; set; }

        public Func<IAdapterConfiguration, IAdapter> Adapter { get; set; }
    }
}