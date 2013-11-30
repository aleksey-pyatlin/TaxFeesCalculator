using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Configuration.Bsb
{
    internal class BsbAdapterConfigurator : IAdapterConfiguration
    {
        public string[] BsbFreeSellPaymentsPath { get; set; }
        public string[] BsbSellPaymentsPath { get; set; }
        public string[] BsbIncommingPaymentsPath { get; set; }
        public string RootFolder { get; set; }
    }
}