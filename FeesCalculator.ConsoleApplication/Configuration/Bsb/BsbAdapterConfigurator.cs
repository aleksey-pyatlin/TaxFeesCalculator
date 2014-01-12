using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Configuration.Bsb
{
    internal class BsbAdapterConfigurator : BaseAdapterConfiguration
    {
        public string[] BsbFreeSellPaymentsPath { get; set; }
        public string[] BsbSellPaymentsPath { get; set; }
        public string[] BsbIncommingPaymentsPath { get; set; }
    }
}