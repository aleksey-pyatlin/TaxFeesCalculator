using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Configuration.Mtb
{
    internal class MtbAdapterConfigurator : IAdapterConfiguration
    {
        public List<string> BsbIncommingPaymentsPath
        {
            get; set;
        }

        public string RootFolder { get; set; }
    }
}