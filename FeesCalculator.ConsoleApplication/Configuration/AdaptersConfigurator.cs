using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class AdaptersConfigurator
    {
        public AdaptersConfigurator()
        {
            Configurations = new List<AdapterConfiguration<BaseAdapterConfiguration>>();
        }

        public List<AdapterConfiguration<BaseAdapterConfiguration>> Configurations { get; set; }
    }
}