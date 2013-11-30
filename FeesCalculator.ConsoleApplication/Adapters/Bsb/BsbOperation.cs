using System;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class BsbOperation
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Destination { get; set; }

        public bool IsUsed { get; set; }
    }
}