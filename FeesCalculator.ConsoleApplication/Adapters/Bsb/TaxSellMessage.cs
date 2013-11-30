using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class TaxSellMessage : OperationMessage
    {
        public QuarterType QuarterType { get; set; }

        public int YearNumber { get; set; }
    }
}