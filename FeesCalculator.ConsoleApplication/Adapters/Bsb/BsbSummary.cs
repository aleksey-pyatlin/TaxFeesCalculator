using System;
using System.Collections.Generic;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class BsbSummary
    {
        public DateTime Date { get; set; }

        public string Content { get; set; }

        public List<BsbOperation> Operations
        {
            get; set; }

      
    }
}