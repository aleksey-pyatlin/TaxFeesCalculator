using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class AdaptersConfigurator
    {
        public AdaptersConfigurator()
        {
            Configurations = new List<AdapterConfiguration<IAdapterConfiguration>>();
        }

        public List<AdapterConfiguration<IAdapterConfiguration>> Configurations { get; set; }
    }
}