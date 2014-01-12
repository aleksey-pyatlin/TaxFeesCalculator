using System;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    [Serializable]
    public class BaseAdapterConfiguration : IAdapterConfiguration
    {
        public String RootFolder { get; set; }
    }
}